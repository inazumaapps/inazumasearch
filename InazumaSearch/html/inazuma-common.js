
// クロール実行前モーダルにフォルダ表示を追加
function addFolderToCrawlModal(path, label, fileCount, checked) {
    var $newItem = $('.folder-item.cloning-base').clone().removeClass('cloning-base');
    $newItem.find('.crawl-modal-folder-check').attr('data-path', path).prop('checked', checked);
    $newItem.find('.path').text(path);
    $newItem.find('.file-count').text(fileCount);

    $('#CRAWL-FOLDER-LIST li.folder-item:last').after($newItem);
    $newItem.show();
}



// クロールダイアログ内、「全選択」チェックの表示更新
function refreshCrawlModalAllCheck() {
    // 全フォルダを選択していれば、「全選択」のチェックをON
    if ($('.crawl-modal-folder-check[data-path]').not(':checked').length === 0) {
        $('#CRAWL-MODAL-ALL-CHECK').prop('checked', true);
    } else {
        $('#CRAWL-MODAL-ALL-CHECK').prop('checked', false);
    }
}

// クロール実行ボタンの表示更新
function refreshCrawlDecideButtonEnabled() {
    // 1つ以上のフォルダを選択していればクロール実行可能
    if ($('.crawl-modal-folder-check[data-path]:checked').length >= 1) {
        $('#CRAWL-MODAL-DECIDE').removeClass('disabled');
    } else {
        $('#CRAWL-MODAL-DECIDE').addClass('disabled');
    }
};

// クロール実行前モーダルの対象フォルダ一覧を更新 (非同期に処理を行う)
function updateFolderListOnCrawlModalAsync() {
    $('#PROGRESS-BAR-IN-CRAWL-MODAL').show();
    $('.folder-item:not(.cloning-base)').remove(); // 既存行の削除
    refreshCrawlDecideButtonEnabled(); // クロール実行ボタンの表示を更新

    // 対象フォルダ一覧取得
    asyncApi.searchTargetDirectories('crawlFolderSelect').then(function (json) {
        var data = JSON.parse(json);
        if (data) {
            for (dir of data.targetDirectories) {
                addFolderToCrawlModal(dir.Path, dir.Label, data.fileCounts[dir.Path], !data.excludingFlags[dir.Path]);
            }
        }
        $('#PROGRESS-BAR-IN-CRAWL-MODAL').hide();

        refreshCrawlModalAllCheck(); // 「全選択」チェックの表示を更新
        refreshCrawlDecideButtonEnabled(); // クロール実行ボタンの表示を更新
    });
}

// クロール開始
function startCrawl(targetFolders = null) {
    // 警告エリアを非表示にする
    $('#MESSAGE-AREA').fadeOut();
    $('.search-button').removeClass('disabled');

    api.crawlStart(JSON.stringify(targetFolders));

    // 0.5秒後、1秒後、1.5秒後、2秒後にそれぞれ最終クロール日時の表示を更新
    // （最終クロール日時の更新は別スレッドで実行されるため、いつ完了するかが分からない）
    for (var delay = 500; delay <= 2000; delay += 500) {
        setTimeout(function () {
            refreshLastCrawlTimeCaption();
        }, delay);
    }
}

// 最終クロール日時の表示を更新
function refreshLastCrawlTimeCaption() {
    $('#LAST-CRAWL-TIME-CAPTION').text(api.getLastCrawlTimeCaption());
}