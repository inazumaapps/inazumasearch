//対象要素と子要素とでの移動フラグ
var innerFlag = false;
//汎用ID番号
var idNumber = 1;

function handleDragEnter(e) {
    e.preventDefault();
    $(this).addClass('dropping');

    innerFlag = true; //移動フラグ

}
function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    //フラグを「握りつぶ」す
    innerFlag = false;

    return false;
}
function handleDragLeave(e) {
    e.preventDefault();


    if (innerFlag) {
        //フラグがセットされている場合、フラグを戻す
        innerFlag = false;
    } else {
        //フラグがセットされていない場合、クラスを削除
        $(this).removeClass('dropping');
    }

}

function handleDrop(e) {
    console.log("handleDrop called.");
    e.stopPropagation(); // Stops some browsers from redirecting.
    e.preventDefault();

    var files = e.dataTransfer.files;
    for (var i = 0, f; f = files[i]; i++) {
        console.log(f);
        console.log(f.path);
        addFolder(f.path, "", "", 0, 0);
    }

    $(this).removeClass('dropping');

    return false;
}

function addFolder(path, label, pathHash, fileCount, ignoreSettingCount) {
    var $newItem = $('.folder-item.cloning-base').clone().removeClass('cloning-base');
    $newItem.attr('data-path-hash', pathHash);
    $newItem.find('.path').text(path);
    $newItem.find('.file-count').text(fileCount);
    $newItem.find('.ignore-setting-count').text(ignoreSettingCount);
    var id = "FOLDER-ITEM-INPUT-" + idNumber.toString();
    $newItem.find('.folder-label-col input').val(label).attr('id', id);
    $newItem.find('.folder-label-col label').attr('for', id);
    idNumber++;

    $('ul.collection li.folder-item:last').after($newItem);
    $newItem.fadeIn();

    $newItem.find('.after-tooltipped').tooltipster();
    M.updateTextFields();
}

// 登録済み文書数、無視設定数のカウントを更新する（非同期に実行）
function updateCountsAsync() {
    asyncApi.searchTargetDirectories().then(function (json) {
        var data = JSON.parse(json);
        if (data) {
            for (dir of data.targetDirectories) {
                // パスが一致する項目を探す
                var $matchedItem = $('.folder-item[data-path-hash="' + data.pathHashes[dir.Path] + '"]')

                // 文書数、無視設定数を更新
                $matchedItem.find('.file-count').text(data.fileCounts[dir.Path]);
                $matchedItem.find('.ignore-setting-count').text(data.ignoreSettingCounts[dir.Path]);
            }
        }
    });
}

$(function () {
    var userSettings = JSON.parse(api.getUserSettings());
    console.log(userSettings);

    // 情報ダイアログ、要望・バグ報告リンクにバージョンを設定
    $('.application-version').text(api.getVersionCaption());
    $('.its-report-link').attr('href', $('.its-report-link').attr('href') + '&issue[custom_field_values][4]=' + api.getVersionCaption());

    // モーダル設定
    $('.modal').modal();

    $('#ABOUT-MODAL-TRIGGER').click(function () {
        $('#ABOUT-MODAL').modal('open');
        return false;
    });

    // 外部リンククリック時処理
    $('.external-link').click(function (e) {
        api.openExternalLink($(e.target).attr('href'));
        return false;
    })

    var updateStartUpEnabled = function () {
        if ($('#ALWAYS-CRAWL-MODE').is(':checked')) {
            $('#STARTUP').removeAttr('disabled');
        } else {
            $('#STARTUP').prop('checked', false);
            $('#STARTUP').attr('disabled', 'disabled');
        }
    }

    $('#ALWAYS-CRAWL-MODE').prop('checked', userSettings.AlwaysCrawlMode);
    $('#STARTUP').prop('checked', userSettings.StartUp);
    updateStartUpEnabled();
    $('#DISPLAY-SEARCH-PROCESS-TIME').prop('checked', userSettings.DisplaySearchProcessTime);

    // ポータブルモードならスタートアップ起動の設定は不可
    if (api.isPortableMode()) {
        $('#STARTUP-AREA').hide();
    }

    // 更新確認に失敗した場合は、メッセージを表示
    if (api.isUpdateCheckFailed()) {
        $('#UPDATE-CHECK-ERROR-MESSAGE').show();
    }

    $('body').on('change', '.folder-label-col input', function () {
        var $input = $(this);
        var $li = $input.closest('li');
        api.updateFolderLabel($li.find('.path').text(), $input.val());
        return false;
    });


    $('#DIPSW-LINK').on('click', function () {
        api.openDipswForm();
        return false;
    });

    $('#ALWAYS-CRAWL-MODE').on('change', function () {
        updateStartUpEnabled();
        asyncApi.changeAlwaysCrawlMode($(this).is(':checked'));
    });
    $('#STARTUP').on('change', function () {
        asyncApi.changeStartUp($(this).is(':checked'));
    });
    $('#DISPLAY-SEARCH-PROCESS-TIME').on('change', function () {
        asyncApi.changeDisplaySearchProcessTime($(this).is(':checked'));
    });

    $('ul.collection').on('click', 'a.delete-link', function () {
        var $li = $(this).closest('li');
        api.deleteTargetDirectory($li.find('.path').text());
        $li.fadeOut();
        return false;
    });

    $('ul.collection').on('click', 'a.ignore-setting-edit-link', function () {
        var $li = $(this).closest('li');
        api.showIgnoreEditFormFromSetting($li.find('.path').text());
        updateCountsAsync(); // 登録済み文書数、無視設定数のカウントを更新
        return false;
    });

    $('a.add-folder-select').on('click', function () {
        var selectedPath = api.selectDirectory();
        if (selectedPath != null) {

            // 対象フォルダ追加
            var resultJson = api.addTargetDirectory(selectedPath);
            if (resultJson != null) {
                var result = JSON.parse(resultJson);
                addFolder(selectedPath, "", result.pathHash, result.fileCount, 0);
            }
        }
        return false;
    });


    // 最初のデータ読み込み
    asyncApi.searchTargetDirectories().then(function (json) {
        var data = JSON.parse(json);
        if (data) {
            for (dir of data.targetDirectories) {
                addFolder(dir.Path, dir.Label, data.pathHashes[dir.Path], data.fileCounts[dir.Path], data.ignoreSettingCounts[dir.Path]);
            }
        }

        // プログレスバーを消す
        $('#PROGRESS-BAR').hide();
    });

});

cols = document.querySelectorAll('.droppable');
[].forEach.call(cols, function (col) {
    console.log("event assigned." + col.toString());
    //col.addEventListener('dragstart', handleDragStart, false);
    col.addEventListener('dragenter', handleDragEnter, false)
    col.addEventListener('dragover', handleDragOver, false);
    col.addEventListener('dragleave', handleDragLeave, false);
    col.addEventListener('drop', handleDrop, false);
    //col.addEventListener('dragend', handleDragEnd, false);
});