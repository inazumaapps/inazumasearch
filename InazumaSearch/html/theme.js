// ダークモードの保存キー
var THEME_STORAGE_KEY = 'inazuma-search-theme';
var DARK_THEME = 'dark';
var LIGHT_THEME = 'light';

/**
 * 保存されたテーマを画面へ反映する。
 * @param {string} theme 表示するテーマ
 */
function applyTheme(theme) {
    var isDarkTheme = theme === DARK_THEME;

    // テーマに応じて画面全体の配色を切り替える
    document.documentElement.classList.toggle('dark-mode', isDarkTheme);

    // 切り替えボタンの表示とアクセシビリティ情報を更新する
    var themeToggle = document.getElementById('THEME-TOGGLE');
    if (themeToggle === null) {
        return;
    }

    themeToggle.setAttribute('aria-pressed', String(isDarkTheme));
    themeToggle.setAttribute('aria-label', isDarkTheme ? 'ライトモードに切り替え' : 'ダークモードに切り替え');
    themeToggle.setAttribute('title', isDarkTheme ? 'ライトモードに切り替え' : 'ダークモードに切り替え');
}

/**
 * 保存されたテーマを取得する。
 * @returns {string} 保存されたテーマ、またはライトテーマ
 */
function getStoredTheme() {
    return localStorage.getItem(THEME_STORAGE_KEY) === DARK_THEME ? DARK_THEME : LIGHT_THEME;
}

/**
 * 現在のテーマを反対のテーマへ切り替える。
 */
function toggleTheme() {
    var nextTheme = document.documentElement.classList.contains('dark-mode') ? LIGHT_THEME : DARK_THEME;

    // 次回の画面表示でも選択状態を復元できるよう保存する
    localStorage.setItem(THEME_STORAGE_KEY, nextTheme);
    applyTheme(nextTheme);
}

// ページ描画前に保存済みテーマを適用してちらつきを抑える
applyTheme(getStoredTheme());

document.addEventListener('DOMContentLoaded', function () {
    // DOM生成後にボタンへ切り替え処理を割り当てる
    applyTheme(getStoredTheme());
    var themeToggle = document.getElementById('THEME-TOGGLE');
    if (themeToggle !== null) {
        themeToggle.addEventListener('click', function (event) {
            event.preventDefault();
            toggleTheme();
        });
    }
});
