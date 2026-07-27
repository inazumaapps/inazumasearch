# Inazuma Search UI tests

CefSharpで表示する検索画面と設定画面の基本動作をChrome DevTools Protocolで確認します。
テスト実行時のDB・設定・ログは一時ディレクトリへ保存され、通常利用のデータには触れません。

## 実行方法

先にInazuma Searchのx64 Debugビルドを作成してから、次のコマンドを実行します。
Node.jsとnpmが必要です。

```powershell
cd InazumaSearchUiTest
npm ci
npm test
```

既定では `InazumaSearch\bin\Debug\x64\InazumaSearch.exe` を起動します。
別の実行ファイルを使用する場合は、`INAZUMA_SEARCH_EXE` に絶対パスを指定してください。

```powershell
$env:INAZUMA_SEARCH_EXE = "C:\path\to\InazumaSearch.exe"
npm test
```

Inazuma Searchがすでに起動している場合は、終了してからテストしてください。
