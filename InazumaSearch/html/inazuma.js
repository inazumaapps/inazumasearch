// persistent variables
var g_lastQueryObject = null;
var g_lastSelectedFormatName = null;
var g_lastSelectedFolderLabel = null;
var g_lastSelectedOrder = null;
var g_lastSelectedView = null;
var g_lastSearchOffset = 0;
var g_searchFinished = false;

// functions
function handleDragOver(e){
    event.preventDefault(); 
}
function handleDrop(e) {
    console.log("handleDrop called.");
    e.stopPropagation(); // Stops some browsers from redirecting.
    e.preventDefault();

    var files = e.dataTransfer.files;
    for (var i = 0, f; f = files[i]; i++) {
        console.log(f);
        console.log(f.path);
    }

    return false;
}

// 検索実行
function executeSearch(
    queryObject
    , hideSearchResult = true
    , selectedFormatName = null
    , selectedFolderLabel = null
    , selectedOrder = null
    , selectedView = null
) {
    var $header = $('#SEARCH-RESULT-HEADER');
    var userSetting = JSON.parse(api.getUserSettings());

    // 変数を初期化
    g_lastSearchOffset = 0;
    g_searchFinished = false;

    // 検索を実行した場合は、更新表示を隠す
    $('#UPDATE-LINK').hide();

    // 既存の検索結果行を削除
    $('.generated-search-result-row').remove();

    // 検索結果部(見出)を隠す
    if (hideSearchResult) $header.css('opacity', '0');

    // 現在のクエリ、選択されたファイル形式とフォルダラベル、次のオフセットを記憶
    g_lastQueryObject = queryObject;
    g_lastSelectedFormatName = selectedFormatName;
    g_lastSelectedFolderLabel = selectedFolderLabel;
    g_lastSelectedOrder = selectedOrder || g_lastSelectedOrder; // 再検索時、並び順は前回と同じ
    g_lastSelectedView = selectedView || g_lastSelectedView; // 再検索時、表示形式は前回と同じ

    // 検索実施
    $('#SEARCH-PROGRESS-BAR').css('opacity', '1');
    asyncApi.search(g_lastQueryObject, true, 0, g_lastSelectedFormatName, g_lastSelectedFolderLabel, g_lastSelectedOrder, g_lastSelectedView).then(function (resJson) {
        var data = JSON.parse(resJson);

        // 検索中表示を消す
        $('#SEARCH-PROGRESS-BAR').css('opacity', '0');

        // 検索結果部(見出)を表示
        if (hideSearchResult) $header.css('opacity', '1');

        $('#SEARCH-RESULT-MESSAGE').text(data.searchResultMessage);
        $('#SEARCH-RESULT-SUB-MESSAGE').text(data.searchResultSubMessage);

        // 全結果の表示が完了していれば、完了フラグを立てる
        if (data.nHits === 0 || data.pageSize >= data.nHits){
            g_searchFinished = true;
        }

        // ドリルダウン結果を表示
        // ただし、ドリルダウンの選択肢が1つだけで、かつ絞り込みしても文書数が変わらない場合は選択肢を表示しない
        if (data.formatDrilldownLinks.length >= 2
            || (data.formatDrilldownLinks.length === 1 && data.formatDrilldownLinks[0].nSubRecs < data.nHits)
            || g_lastSelectedFormatName) {
            var resHtml = "ファイル形式で絞り込む: ";
            for(var link of data.formatDrilldownLinks){
                if(g_lastSelectedFormatName === link.name){ // 現在選択中の絞り込み
                    resHtml += '<span class="selected-drilldown">[' + link.caption + ']</span> ';
                } else if (!g_lastSelectedFormatName) { // 絞り込みを行っていない場合のみ、選択肢を表示
                    resHtml += '<a href="#" class="drilldown-ext-link" data-value="' + link.name + '">' + link.caption + '(' + link.nSubRecs + ')</a> ';
                }
            }
            if(g_lastSelectedFormatName !== null){
                resHtml += '<a href="#" class="drilldown-ext-link" data-value="">解除</a> ';
            }
            $('#DRILLDOWN-RESULT-EXT').html(resHtml);
        } else {
            $('#DRILLDOWN-RESULT-EXT').html("");
        }


        if (data.folderLabelDrilldownLinks.length >= 2
            || (data.folderLabelDrilldownLinks.length === 1 && data.folderLabelDrilldownLinks[0].nSubRecs < data.nHits)
            || g_lastSelectedFolderLabel) {
            var resHtmlFolderLabel = "フォルダラベルで絞り込む: ";
            for(var link of data.folderLabelDrilldownLinks){
                if (g_lastSelectedFolderLabel === link.folderLabel) { // 現在選択中の絞り込み
                    resHtml += '<span class="selected-drilldown">[' + link.caption + ']</span> ';
                } else if (!g_lastSelectedFolderLabel) { // 絞り込みを行っていない場合のみ、選択肢を表示
                    resHtmlFolderLabel += '<a href="#" class="drilldown-folder-label-link" data-value="' + link.folderLabel + '">' + link.folderLabel + '(' + link.nSubRecs + ')</a> ';
                }
            }
            if(g_lastSelectedFolderLabel !== null){
                resHtmlFolderLabel += '<a href="#" class="drilldown-folder-label-link" data-value="">解除</a> ';
            }
            $('#DRILLDOWN-RESULT-FOLDER-LABEL').html(resHtmlFolderLabel);
        } else {
            $('#DRILLDOWN-RESULT-FOLDER-LABEL').html("");
        }

        // 検索結果が1件以上であれば、並び順と表示形式の選択肢を表示
        if (data.nHits >= 1){
            var resHtml = '<div class="input-field inline" style="margin-top: 0;">';
            resHtml += '<select class="browser-default sort-select" style="height: 2rem; padding: 2px; border-color: silver; display: inline-block; margin-right: 0.3rem; width: auto;">';
            for (var order of data.orderList) {
                var selected = (g_lastSelectedOrder === order.Type || (g_lastSelectedOrder === null && order.Type === 'score'));
                resHtml += '<option value="' + order.Type + '" ' + (selected ? 'selected' : '') + '>' + order.Caption + 'で</option> ';
            }
            resHtml += "</select>"

            resHtml += '<select class="browser-default view-select" style="height: 2rem; padding: 2px; border-color: silver; display: inline-block; width: auto;">';
            resHtml += '<option value="normal" ' + (g_lastSelectedView !== 'list' ? 'selected' : '') + '>通常表示</option> ';
            resHtml += '<option value="list" ' + (g_lastSelectedView === 'list' ? 'selected' : '') + '>一覧表示</option> ';
            resHtml += "</select>";
            resHtml += "</div > "
            $('#DRILLDOWN-ORDER').html(resHtml);
        }

        // 検索結果の各行を表示
        displayResultRows(data, g_lastSelectedView);
    });
}

// 結果リストを表示する
function displayResultRows(getJsonData, selectedView, searchOffset = 0){
    if (selectedView === 'list'){
        displayResultRows_ListView(getJsonData, searchOffset);
    } else {
        displayResultRows_NormalView(getJsonData, searchOffset);
    }

    // 残りの結果があるかどうかを判定し、ローディングアイコンを表示
    if(g_searchFinished){
        $('#SEARCH-RESULT-LOADING').hide();
    } else {
        $('#SEARCH-RESULT-LOADING').show();
    }

    // イベントやプラグインの登録
    $('[data-search-offset=' + searchOffset + '] .after-tooltipped').tooltipster();
    $('[data-search-offset=' + searchOffset + '] a.file-open-link').click(function(){
        var path = $(this).attr('data-file-path');
        api.openFile(path);
        return false;
    });

    $('[data-search-offset=' + searchOffset + '] a.folder-open-link').click(function(){
        var path = $(this).attr('data-file-path');
        api.openFolder(path);
        return false;
    });

}

// 結果リストを表示する(通常モード)
function displayResultRows_NormalView(getJsonData, searchOffset){
    // 通常表示用の結果エリアを表示
    $('#SEARCH-RESULT-LIST-VIEW-BODY').show();
    // 一覧表示用の結果エリアを隠す
    $('#SEARCH-RESULT-LIST-VIEW-BODY').hide();

    var $row_base = $('#RESULT-ROW-BASE');
    for(var i = 0; i < getJsonData.records.length; i++){
        var res = getJsonData.records[i];

        var $new_row = $row_base.clone();

        var title;
        if (res.title_snippets.length >= 1) {
            title = res.title_snippets[0];
        } else if (res.title !== null && res.title !== "") {
            title = res.title;
        } else if(res.file_name_snippets.length >= 1) {
            title = res.file_name_snippets[0];
        } else {
            title = res.file_name;
        }
        $new_row.find('.card-title a').html(title).attr('data-file-path', res.file_path);
        var fileLinkHref = '#FILE:' + res.file_path ;
        $new_row.find('.card-title a').attr('href', fileLinkHref);
        $new_row.find('.card-action a.file-path').text(res.file_path).attr('href', fileLinkHref).attr('data-file-path', res.file_path);
        $new_row.find('.card-action a.folder-open-link').attr('data-file-path', res.file_path);
        if(res.body_snippets.length >= 1){
            res.body_snippets.forEach(function(snip){
                $new_row.find('.body-snippets').append('<div style="border: 1px solid #f0f0f0; margin: 1em 0; padding: 1em; font-size: small;">' + snip + '</div>');
            });
     
        } else {
            $new_row.find('.body-snippets').remove();
        }

        $new_row.find('.document-information-size').text(res.size_caption);
        $new_row.find('.document-information-file-updated').text(res.timestamp_updated_caption);
        $new_row.find('.document-information-score').text(res.final_score);

        $new_row.attr('data-key', res.key);
        $new_row.find('.submenu-link').dropdown({ constrainWidth: false, container: $('#SEARCH-RESULT-BODY').get(0), alignment: 'right' });

        // $new_row.find('.display-similar-documents').attr('data-key', res.key);

        $new_row.addClass('file-type-' + res.ext);
        $new_row.find('.icon').removeClass().addClass('icon').addClass('file-type-' + res.ext);
        ///$new_row.find('.icon').html('<img src="' + res.icon_data_url + '" />');

        //   $.get('internal://thumbnail/test2.png').then(function(data){
        //     console.log('image loaded.');
        //  });

        if(res.thumbnail_path != null){
            $new_row.find('.thumbnail-image').attr('src', res.thumbnail_path);
        } else {
            $new_row.find('.thumbnail-image').hide();
        }

        $new_row.attr('data-file-path', res.file_path);

        $new_row.show();
        $new_row.attr('id', 'RESULT-ROW-' + (searchOffset + i)).addClass('generated-search-result-row');
        $('#SEARCH-RESULT-BODY').append($new_row);
        $new_row.attr('data-search-offset', searchOffset);
        $new_row.css('position', 'relative').css('left', '200px');
    
    }

    // 一番下の要素と同じ縦位置に、スクロール補正用要素を移動
    var $lastRow = $('.generated-search-result-row:last');
    if($lastRow.length >= 1){
        $('#SCROLL-ADJUSTER').offset({top: $lastRow.offset().top + $lastRow.height()}).show();
    } else {
        $('#SCROLL-ADJUSTER').hide();

    }

    // アニメーション
    var displayCardIndex = searchOffset;
    var cardDisplayCallback = function(){
        $('#RESULT-ROW-' + displayCardIndex).css('opacity', '1').css('left', '0');
        displayCardIndex++;

        if($('#RESULT-ROW-' + displayCardIndex).length >= 1){
            setTimeout(cardDisplayCallback, 200);
        }
    };

    setTimeout(cardDisplayCallback, 200);

}


// 結果リストを表示する (一覧表示モード)
function displayResultRows_ListView(getJsonData, searchOffset){
    // 通常表示用の結果エリアを隠す
    $('#SEARCH-RESULT-LIST-VIEW-BODY').hide();
    // 一覧表示用の結果エリアを表示
    $('#SEARCH-RESULT-LIST-VIEW-BODY').show();

    var $row_base = $('#RESULT-LIST-VIEW-ROW-BASE');
    console.log(getJsonData);
    for(var i = 0; i < getJsonData.records.length; i++){
        var res = getJsonData.records[i];
        var $new_row = $row_base.clone();

        var file_name;
        if(res.file_name_snippets.length >= 1){
            file_name = res.file_name_snippets[0];
        } else {
            file_name = res.file_name;
        }
        $new_row.find('.file-name-link').html(file_name).attr('data-file-path', res.file_path);
        var fileLinkHref = '#FILE:' + res.file_path ;
        $new_row.find('.file-name-link').attr('href', fileLinkHref);
        $new_row.find('.file-path').text(res.file_path);
        $new_row.find('a.folder-open-link').attr('data-file-path', res.file_path);

        $new_row.find('.document-information-size').text(res.size_caption);
        $new_row.find('.document-information-file-updated').text(res.timestamp_updated_caption_for_list_view);
        $new_row.find('.document-information-score').text(res.final_score);

        $new_row.addClass('file-type-' + res.ext);
        $new_row.find('.icon').removeClass().addClass('icon').addClass('file-type-' + res.ext);

        $new_row.show();
        $new_row.attr('id', 'RESULT-ROW-' + (searchOffset + i)).addClass('generated-search-result-row');
        $('#SEARCH-RESULT-LIST-VIEW-BODY tbody').append($new_row);
        $new_row.attr('data-search-offset', searchOffset);
    }

    // 一番下の要素と同じ縦位置に、スクロール補正用要素を移動
    var $lastRow = $('.generated-search-result-row:last');
    if($lastRow.length >= 1){
        $('#SCROLL-ADJUSTER').offset({top: $lastRow.offset().top + $lastRow.height()}).show();
    } else {
        $('#SCROLL-ADJUSTER').hide();
    }

}

// クロール実行前モーダルにフォルダ表示を追加
function addFolderToCrawlModal(path, label, fileCount, checked) {
    var $newItem = $('.folder-item.cloning-base').clone().removeClass('cloning-base');
    $newItem.find('.crawl-modal-folder-check').attr('data-path', path).prop('checked', checked);
    $newItem.find('.path').text(path);
    $newItem.find('.file-count').text(fileCount);
    if (label) {
        $newItem.find('.folder-label').text(`(${label})`);
    }

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
    $('#CRAWL-MODAL-ALL-CHECK-AREA').hide();
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
        $('#CRAWL-MODAL-ALL-CHECK-AREA').show();

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

cols = document.querySelectorAll('.droppable');
[].forEach.call(cols, function(col) {
    console.log("event assigned." + col.toString());
    //col.addEventListener('dragstart', handleDragStart, false);
    //col.addEventListener('dragenter', handleDragEnter, false)
    //col.addEventListener('dragleave', handleDragLeave, false);
    col.addEventListener('drop', handleDrop, false);
    //col.addEventListener('dragend', handleDragEnd, false);
});


$(async function () {
    await CefSharp.BindObjectAsync("api", "asyncApi", "dbState");

    if (api.isDebugMode()) {
        $('.debug-mode-only').show();
        $('.release-mode-only').hide();
    }

    // 入力情報のクリア
    asyncApi.clearUserInputLog();

    //$('#KEYWORD-INPUT').autocomplete({
    //    limit: 20
    //})

    var backgroundSearchTimeoutHandle = null;
    var keywordOnLastKeydown = '';
    $('#KEYWORD-INPUT').on('keyup', function(e){
        var $input = $(this);
        var value = $input.val();

        asyncApi.addUserInputLog(new Date(), value);

        //asyncApi.getAutoCompleteData(value).then(function(dataJson){
        //    var data = JSON.parse(dataJson);
        //    $input.autocomplete('updateData', data);
        //});

        // テキストの値が変わった場合で、詳細検索をONにしていない場合は、バックグラウンド検索を実行
        var detailSearchFlag = $('#DETAIL-SEARCH-SWITCH input:checkbox').is(':checked');
        if(value !== keywordOnLastKeydown && !detailSearchFlag){
            // 前回予約した検索処理をクリア
            if(backgroundSearchTimeoutHandle) clearTimeout(backgroundSearchTimeoutHandle);

            // 値が空文字列かどうかで分岐
            if(value === ''){
                // 空文字列なら、表示をクリア
                $('#BACKGROUND-SEARCH-RESULT').text("");
            } else {
                // 空文字列でなければ、検索を予約
                $('#BACKGROUND-SEARCH-RESULT').text("条件に合致する文書数: ");

                backgroundSearchTimeoutHandle = setTimeout(function(){
                    // 検索リクエストを実行
                    var queryObject = {
                        keyword: value
                        , fileName: ''
                        , body: ''
                        , updated: ''
                        , tfIdf: ''
                    }

                    asyncApi.search(queryObject, false, 0, null, null, null, null, true).then(function (resJson) {
                        if (resJson) {
                            var data = JSON.parse(resJson);

                            $('#BACKGROUND-SEARCH-RESULT').text("条件に合致する文書数: " + data.nHits.toString());
                        } else {
                            $('#BACKGROUND-SEARCH-RESULT').text("条件に合致する文書数: 不明");
                        }
                    });

                }, 500)
            }


            // 変更後のテキストの値をセット
            keywordOnLastKeydown = value;
        }
    })


    $('#DISPLAY-HIGHLIGHT-MODAL').modal();
    $('#QUERY-GUIDE-MODAL').modal();
    $('#CRAWL-MODAL').modal({
        onCloseEnd: function () {
            // クロール実行ボタンを押していればクロール処理
            if ($('#CRAWL-MODAL').attr('data-decide-flag') === '1') {
                // チェックしているフォルダのパスを取得
                var targetFolders = [];
                $('.crawl-modal-folder-check:checked[data-path]').each(function () {
                    targetFolders.push($(this).attr('data-path'));
                });
                startCrawl(targetFolders);
            }
        }
    });

    let lastClickedPath;
    let lastClickedKey;
    $('#SEARCH-RESULT-BODY').on('click', '.submenu-link', function (e) {
        //var instance = M.Dropdown.init(e.target, { constrainWidth: false, container: $('body').get(0) });
        //instance.open();
        var $row = $(this).closest('.search-result-row');
        lastClickedPath = $row.attr('data-file-path');
        lastClickedKey = $row.attr('data-key');
        $(e.target).closest('.submenu-link').dropdown('open');
        return false;
    });

    // 無視設定ダイアログ表示
    $('body').on('click', '.ignore-dialog-link', function () {
        api.showIgnoreEditFormFromSearchResult(lastClickedPath);
        return false;
    });

    // マッチ箇所の全表示
    $('body').on('click', '.display-all-body-link', function () {
        $('#DISPLAY-HIGHLIGHT-MODAL .loading').show();
        $('#DISPLAY-HIGHLIGHT-MODAL .result').hide();
        $('#DISPLAY-HIGHLIGHT-MODAL').modal('open');
        $('#DISPLAY-HIGHLIGHT-MODAL .modal-content').scrollTop(0); // なぜかopenの後にスクロールを設定しないと、正常にスクロール位置が移動しない

        // 1度実行してからモーダルを閉じ、その後1度目の処理が終了する前に別のマッチ箇所表示を実行すると
        // 1度目の処理結果が誤ってセットされることがある
        // これを防ぐために、実行ごとにワンタイムトークンを発行
        let token = Math.ceil(Math.random() * 10000).toString();
        $('#DISPLAY-HIGHLIGHT-MODAL').attr('data-token', token);

        asyncApi.getHighlightedBody(lastClickedKey, g_lastQueryObject.keyword, g_lastQueryObject.body).then(function (resJson) {
            if ($('#DISPLAY-HIGHLIGHT-MODAL').attr('data-token') === token) {
                var res = JSON.parse(resJson);
                if (res !== null) {
                    $('#DISPLAY-HIGHLIGHT-MODAL .loading').hide();
                    $('#DISPLAY-HIGHLIGHT-MODAL .result').show();
                    $('.display-highlight-body').html(res.body);
                    $('.body-match-count').text(res.hitCount);
                } else {
                    api.showErrorMessage("本文にマッチしていません。");
                    $('#DISPLAY-HIGHLIGHT-MODAL').modal('close');
                }
            }
        });
        return false;
    });

    // ファイル本文の取得（デバッグ用）
    $('body').on('click', '.get-body-link', function () {
        asyncApi.getFileBody(lastClickedPath);
        return false;
    });

    $('#UPDATE-LINK').click(function () {
        api.showUpdateForm();
        return false;
    });

    // $('#SEARCH-RESULT-BODY').on('click', '.display-similar-documents', function(){
    //   var key = $(this).attr('data-key');
    //   asyncApi.getSimilarDocuments(key).then(function(resJson){
    //     var res = JSON.parse(resJson);
    //     console.log(res);
    //   });
    //   return false;
    // });

    $('#QUERY-GUIDE-MODAL-TRIGGER').click(function(){
        $('#QUERY-GUIDE-MODAL').modal('open');
        return false;
    });

    $('select').formSelect();
    $('.tooltipped').tooltipster();

    $('#CRAWL-START').click(function () {
        // 検索対象フォルダが2件以上かどうかで処理を分岐
        if (dbState.targetFolderCount >= 2) {
            // 2件以上ならダイアログを開いて、クロールするフォルダを選択
            updateFolderListOnCrawlModalAsync(); // フォルダリスト更新
            $('#CRAWL-MODAL').removeAttr('data-decide-flag'); // 確定フラグ初期化
            var modal = M.Modal.getInstance($('#CRAWL-MODAL')[0]);
            modal.open();
        } else {
            // 1件ならそのままクロール実行
            startCrawl();
        }
    });

    // クロールダイアログで「全選択」ボタンを押下
    $('#CRAWL-MODAL-ALL-CHECK').click(function (e) {
        if ($(this).prop('checked')) {
            $('.crawl-modal-folder-check[data-path]').prop('checked', true);
        } else {
            $('.crawl-modal-folder-check[data-path]').prop('checked', false);
        }

        refreshCrawlDecideButtonEnabled(); // クロール実行ボタンの表示を更新
    });
    // クロールダイアログで、各フォルダのチェックボックスを変更
    $('#CRAWL-MODAL').on('click', '.crawl-modal-folder-check', function (e) {
        refreshCrawlModalAllCheck(); // 「全選択」チェックの表示を更新
        refreshCrawlDecideButtonEnabled(); // クロール実行ボタンの表示を更新
    });

    // クロールダイアログで「クロール実行」ボタンを押下
    $('#CRAWL-MODAL-DECIDE').click(function () {
        $('#CRAWL-MODAL').attr('data-decide-flag', '1');
        $('#CRAWL-MODAL').modal('close');
    });

    $('#TEST-NOTIFY').click(function(){
        new Notification("通知起動");
    });

    $('#DETAIL-SEARCH-SWITCH input:checkbox').click(function(){
        var $check = $(this);

        if($check.is(':checked')){
            $('#DETAIL-SEARCH').slideDown();
        } else {
            $('#DETAIL-SEARCH').slideUp();
        }

        // バックグラウンド検索の結果をクリアし、実行中のバックグラウンド検索があれば止める
        $('#BACKGROUND-SEARCH-RESULT').text("");
        if(backgroundSearchTimeoutHandle) clearTimeout(backgroundSearchTimeoutHandle);
    });


    // 検索ボタンクリック
    $('.search-button').click(function(){
        // キーワード、ファイル名、本文の入力値取得。空であれば何もしない
        var keyword = $('input[name=keyword]').val();
        var file_name = $('input[name=file_name]').val();
        var body = $('input[name=body]').val();
        var updated = $('select[name=updated]').val();

        // 詳細検索OFFの場合はキーワード以外を反映しない
        var detailSearchFlag = $('#DETAIL-SEARCH-SWITCH input:checkbox').is(':checked');
        if(!detailSearchFlag){
            file_name = '';
            body = '';
            updated = '';
        }

        // 検索語が入力されていなければエラー
        if(keyword === '' && file_name === '' && body === ''){
            api.showErrorMessage(detailSearchFlag ? '検索キーワード、ファイル名、本文のいずれかを入力してください。' : '検索キーワードを入力してください。');
            return false;
        }

        // 検索リクエストを実行
        var queryObject = {
            keyword: keyword
            , fileName: file_name
            , body: body
            , updated: updated
            , tfIdf: $('input:checkbox[name=tf_idf]').is(':checked')
        }
        executeSearch(queryObject);

        // バックグラウンド検索の結果をクリアし、実行中のバックグラウンド検索があれば止める
        $('#BACKGROUND-SEARCH-RESULT').text("");
        if(backgroundSearchTimeoutHandle) clearTimeout(backgroundSearchTimeoutHandle);

        return false;

    });

    // ドリルダウンクリック
    $('#SEARCH-RESULT-HEADER').on('click', '.drilldown-ext-link', function(){
        var formatName = $(this).attr('data-value');
        if (formatName === '') formatName = null; // 解除

        // 再検索
        executeSearch(g_lastQueryObject, true, formatName, g_lastSelectedFolderLabel || null, g_lastSelectedOrder || null, g_lastSelectedView || null);

        return false;
    });

    $('#SEARCH-RESULT-HEADER').on('click', '.drilldown-folder-label-link', function(){
        var folderLabel = $(this).attr('data-value');
        if (folderLabel === '') folderLabel = null; // 解除

        // 再検索
        executeSearch(g_lastQueryObject, true, g_lastSelectedFormatName || null, folderLabel, g_lastSelectedOrder || null, g_lastSelectedView || null);

        return false;
    });

    $('#DRILLDOWN-ORDER').on('change', '.sort-select', function () {
        var orderType = $(this).val();
        // 再検索 (検索結果見出部は非表示にしない)
        executeSearch(g_lastQueryObject, false, g_lastSelectedFormatName || null, g_lastSelectedFolderLabel || null, orderType, g_lastSelectedView || null);

        return false;
    });

    $('#DRILLDOWN-ORDER').on('change', '.view-select', function () {
        var viewType = $(this).val();
        // 再検索 (検索結果見出部は非表示にしない)
        executeSearch(g_lastQueryObject, false, g_lastSelectedFormatName || null, g_lastSelectedFolderLabel || null, g_lastSelectedOrder || null, viewType);

        return false;
    });

    $('#DEVTOOL').click(function(){
        api.showDevTool();
    });

    // DBの状態に応じたUI制御
    if(dbState.targetFolderCount == 0){
        // 対象フォルダ登録なし
        $('#MESSAGE-AREA').show();
        $('#FOLDER-ADD-REQUIRED-MESSAGE').show();
        $('#CRAWL-START').addClass('disabled');
        $('.search-button').addClass('disabled');
    } else {
        // 対象フォルダ登録あり
        if(dbState.documentCount == 0 && !dbState.alwaysCrawlMode){
            // クロール済み文書なし、かつ常駐クロールモードでない
            $('#MESSAGE-AREA').show();
            $('#CRAWL-REQUIRED-MESSAGE').show();
            $('.search-button').addClass('disabled');
        } else {
            // 検索可能
        }
    }

    // 最終クロール日時の表示を更新
    refreshLastCrawlTimeCaption();

    // Each time the user scrolls
    var win = $(window);
    win.scroll(function () {
        if (location.href.endsWith('index.html')) {
            // End of the document reached?
            if (($(document).height() - win.height() - 100) <= win.scrollTop()) {
                var userSetting = JSON.parse(api.getUserSettings());
                var pageSize = (g_lastSelectedView === 'list' ? userSetting.DisplayPageSizeForListView : userSetting.DisplayPageSizeForNormalView);
                var offset = g_lastSearchOffset + pageSize;

                if (offset > g_lastSearchOffset && !g_searchFinished) {
                    g_lastSearchOffset = offset;

                    asyncApi.search(g_lastQueryObject, false, offset, g_lastSelectedFormatName, g_lastSelectedFolderLabel, g_lastSelectedOrder, g_lastSelectedView).then(function (resJson) {
                        var data = JSON.parse(resJson);

                        // 全結果の表示が完了していれば、完了フラグを立てる
                        if (offset + data.pageSize >= data.nHits) {
                            g_searchFinished = true;
                        }

                        // 検索結果の各行を表示
                        displayResultRows(data, g_lastSelectedView, offset);
                    });
                }
            }
        }
    });
});

