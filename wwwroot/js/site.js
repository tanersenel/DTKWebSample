// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// camera stream video element
let on_stream_video = document.querySelector('#camera-stream');
// flip button element
let flipBtn = document.querySelector('#flip-btn');

// default user media options
let constraints = { audio: false, video: true }
let shouldFaceUser = true;

// check whether we can use facingMode
let supports = navigator.mediaDevices.getSupportedConstraints();
if (supports['facingMode'] === true) {
    flipBtn.disabled = false;
}

let stream = null;

function capture() {
    constraints.video = {
        width: {
            min: 600,
            ideal: 600,
            max: 600,
        },
        height: {
            min: 800,
            ideal: 800,
            max: 800
        },
        facingMode: shouldFaceUser ? 'user' : 'environment'
    }
    navigator.mediaDevices.getUserMedia(constraints)
        .then(function (mediaStream) {
            stream = mediaStream;
            on_stream_video.srcObject = stream;
            on_stream_video.play();
        })
        .catch(function (err) {
            console.log(err)
        });
}

flipBtn.addEventListener('click', function () {
    if (stream == null) return
    // we need to flip, stop everything
    stream.getTracks().forEach(t => {
        t.stop();
    });
    // toggle / flip
    shouldFaceUser = !shouldFaceUser;
    capture();
})

capture();
function uploadFile() {
    var fd = new FormData();
    var files = $('#file')[0].files[0];
    fd.append('file', files);
    $('#loader').show();
    $('#imgPlateUrl').attr('src','');
    $('#imgFileUrl').attr('src', '');
    $('#plate').html('');
    $('#message').html('');
    $.ajax({
        url: '/Home/UploadCam',
        type: 'post',
        data: fd,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.status == false) {
                $('#message').html(response.error);
            }
            else {
                $('#imgPlateUrl').attr('src', response.platePhoto);
                $('#imgFileUrl').attr('src', response.uploadedPhoto);
                $('#plate').html(response.plate);
            }
            $('#loader').hide();
        },
    });
    
    return false;
}
document.getElementById("capture-camera").addEventListener("click", function () {
    const video = document.querySelector('video');
    canvas.width = 600;
    canvas.height = 800;
    canvas.getContext('2d').drawImage(video, 0, 0);
    let image_data_url = document.querySelector("#canvas").toDataURL();
    console.log(image_data_url);

    let file = null;
    let blob = document.querySelector("#canvas").toBlob(function (blob) {
        file = new File([blob], 'test.png', { type: 'image/png' });
        console.log(file.name);
        $('#loader').show();
        $('#imgPlateUrl').attr('src', '');
        $('#imgFileUrl').attr('src', '');
        $('#plate').html('');
        $('#message').html('');

        var fd = new FormData();
        fd.append('file', file);

        $.ajax({
            url: '/Home/UploadCam',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.status==false) {
                    $('#message').html(response.error);
                }
                else {
                    $('#imgPlateUrl').attr('src', response.platePhoto);
                    $('#imgFileUrl').attr('src', response.uploadedPhoto);
                    $('#plate').html(response.plate);

                }
                $('#loader').hide();
            },
        });
        

    }, 'image/png');
    console.log(file);
});