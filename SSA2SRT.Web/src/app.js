import 'bootstrap/dist/js/bootstrap.js';
import "./bootstrap-fileinput/js/plugins/piexif.js";
import "./bootstrap-fileinput/js/plugins/sortable.js";
import "./bootstrap-fileinput/js/fileinput.js";
import "./bootstrap-fileinput/themes/explorer-fas/theme.js";

import "bootstrap/dist/css/bootstrap.css";
import "./fontawesome-free-5.15.2-web/css/fontawesome.css";
import "./fontawesome-free-5.15.2-web/css/solid.css";
import "./bootstrap-fileinput/css/fileinput.css";
import "./bootstrap-fileinput/themes/explorer-fas/theme.css";
import "./styles/app.css";

function initFileInput(element) {
    const filesinput = $(element);

    filesinput.fileinput({
        theme: "explorer-fas",
        uploadUrl: "/SSA2SRT/Home/Convert",
        uploadAsync: false,
        showUpload: true,
        allowedFileExtensions: ["zip", "ssa", "ass"],
        removeFromPreviewOnError: true,
        overwriteInitial: false,
        previewFileIcon: '<i class="fas fa-file"></i>',
        preferIconicPreview: true,
        previewFileIconSettings: {
            'zip': '<i class="fas fa-file-archive text-muted"></i>',
            'ssa': '<i class="fas fa-file-code text-primary"></i>'
        },
        previewFileExtSettings: {
            'zip': function (ext) {
                return ext.match(/(zip)$/i);
            },
            'ssa': function (ext) {
                return ext.match(/(ssa|ass)$/i);
            }
        },
        fileActionSettings: {
            showPreview: false,
            showZoom: false
        }
    });

    function save(data) {
        var sa = document.createElement('a');

        if (data.response["isZip"]) {

            sa.setAttribute('href', 'data:application/zip;base64,' + data.response["fileData"]);
        }
        else {
            sa.setAttribute('href', 'data:text/plain;base64,' + data.response["fileData"]);
        }

        sa.setAttribute('download', data.response["fileName"]);
        sa.click();
    }

    filesinput.on('filebatchuploadsuccess', function (event, data) {
        save(data);
    });

    filesinput.on('fileuploaded', function (event, data, previewId, index) {
        save(data);
    });
}

window.initFileInput = initFileInput;