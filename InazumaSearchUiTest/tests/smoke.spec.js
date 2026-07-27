const assert = require('node:assert/strict');
const { execFile, spawn } = require('node:child_process');
const fs = require('node:fs');
const net = require('node:net');
const os = require('node:os');
const path = require('node:path');
const { after, before, test } = require('node:test');
const CDP = require('chrome-remote-interface');

const projectRoot = path.resolve(__dirname, '..', '..');
const appPath = process.env.INAZUMA_SEARCH_EXE
  || path.join(projectRoot, 'InazumaSearch', 'bin', 'Debug', 'x64', 'InazumaSearch.exe');
const htmlPath = path.join(projectRoot, 'InazumaSearch', 'html');

let appProcess;
let client;
let testDataPath;

/**
 * 未使用のローカルポートを取得する。
 */
async function getUnusedPort() {
  return await new Promise((resolve, reject) => {
    const server = net.createServer();
    server.once('error', reject);
    server.listen(0, '127.0.0.1', () => {
      const { port } = server.address();
      server.close(() => resolve(port));
    });
  });
}

/**
 * CefSharpのデバッグ対象ページが取得できるまで待機する。
 */
async function waitForPageTarget(port) {
  const timeoutAt = Date.now() + 30000;

  while (Date.now() < timeoutAt) {
    if (appProcess.exitCode !== null) {
      throw new Error(`Inazuma Search exited before the UI became ready. Exit code: ${appProcess.exitCode}`);
    }

    try {
      const targets = await CDP.List({ host: '127.0.0.1', port });
      const pageTarget = targets.find((target) => (
        target.type === 'page' && target.url.endsWith('index.html')
      ));
      if (pageTarget) {
        return pageTarget;
      }
    } catch {
      // アプリケーションの起動完了まで再試行
    }

    await new Promise((resolve) => setTimeout(resolve, 250));
  }

  throw new Error(`Timed out waiting for the Inazuma Search UI on port ${port}`);
}

/**
 * JavaScriptを画面内で実行して戻り値を取得する。
 */
async function evaluate(expression) {
  const result = await client.Runtime.evaluate({
    expression,
    awaitPromise: true,
    returnByValue: true,
  });

  if (result.exceptionDetails) {
    throw new Error(result.exceptionDetails.text);
  }

  return result.result.value;
}

/**
 * 指定した画面条件が成立するまで待機する。
 */
async function waitForCondition(expression, message) {
  const timeoutAt = Date.now() + 10000;

  while (Date.now() < timeoutAt) {
    if (await evaluate(expression)) {
      return;
    }
    await new Promise((resolve) => setTimeout(resolve, 100));
  }

  throw new Error(message);
}

/**
 * アプリケーションを終了する。
 */
async function stopApplication() {
  if (!appProcess || appProcess.exitCode !== null) {
    return;
  }

  // CefSharpの子プロセスも含めて終了
  await new Promise((resolve) => {
    execFile('taskkill', ['/PID', String(appProcess.pid), '/T', '/F'], () => resolve());
  });
  await new Promise((resolve) => {
    if (appProcess.exitCode !== null) {
      resolve();
      return;
    }

    appProcess.once('exit', resolve);
    setTimeout(resolve, 5000);
  });
}

before(async () => {
  if (!fs.existsSync(appPath)) {
    throw new Error(`Inazuma Search executable was not found: ${appPath}`);
  }

  // 実利用データと分離した状態でアプリケーションを起動
  const port = await getUnusedPort();
  testDataPath = fs.mkdtempSync(path.join(os.tmpdir(), 'inazumasearch-ui-test-'));
  appProcess = spawn(appPath, [
    '--html-path', htmlPath,
    '--data-path', testDataPath,
    '--remote-debugging-port', String(port),
  ], {
    cwd: path.dirname(appPath),
    stdio: 'ignore',
  });

  // 埋め込みChromiumへ接続
  const pageTarget = await waitForPageTarget(port);
  client = await CDP({
    host: '127.0.0.1',
    port,
    target: pageTarget,
  });
  await client.Page.enable();
  await client.Runtime.enable();
  await waitForCondition(
    'document.readyState === "complete" && document.querySelector("#KEYWORD-INPUT") !== null',
    '検索画面の読み込みが完了しませんでした。',
  );
});

after(async () => {
  if (client) {
    await client.close();
  }
  await stopApplication();
  if (testDataPath) {
    fs.rmSync(testDataPath, {
      recursive: true,
      force: true,
      maxRetries: 5,
      retryDelay: 200,
    });
  }
});

test('検索画面でキーワードを入力できる', async () => {
  assert.equal(await evaluate('document.querySelector("#KEYWORD-INPUT") !== null'), true);

  // 検索処理を実行せず、基本的な入力動作だけを確認
  await evaluate(`
    (() => {
      const input = document.querySelector('#KEYWORD-INPUT');
      input.value = '自動テスト';
      input.dispatchEvent(new Event('input', { bubbles: true }));
    })()
  `);
  assert.equal(await evaluate('document.querySelector("#KEYWORD-INPUT").value'), '自動テスト');
  assert.equal(await evaluate(`
    typeof api === 'object'
      && typeof asyncApi === 'object'
      && typeof dbState === 'object'
  `), true);
});

test('設定画面へ移動して検索画面へ戻れる', async () => {
  await evaluate('document.querySelector("#SETTING-LINK").click()');
  await waitForCondition(
    'location.pathname.endsWith("setting.html") && document.querySelector(".add-folder-select") !== null',
    '設定画面へ移動できませんでした。',
  );
  assert.equal(await evaluate('document.querySelector("#ALWAYS-CRAWL-MODE") !== null'), true);

  // 検索画面へ戻り、主要入力欄が再表示されることを確認
  await evaluate(`
    Array.from(document.querySelectorAll('a'))
      .find((link) => link.getAttribute('href') === 'index.html')
      .click()
  `);
  await waitForCondition(
    'location.pathname.endsWith("index.html") && document.querySelector("#KEYWORD-INPUT") !== null',
    '検索画面へ戻れませんでした。',
  );
});
