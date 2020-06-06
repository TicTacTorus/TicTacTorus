window.onresize = stretch_first_canvas;

function client_width() {
    return $(window).width();
}
function client_height() {
    return $(window).height();
}

function stretch_first_canvas()
{
    let canvas = document.querySelector("canvas");
    canvas.style.width = "100%";
    canvas.style.height = "100%";

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;

    DotNet.invokeMethodAsync("TicTacTorus", "RedrawGame");
}