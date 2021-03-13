// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', function (e) {

    // e.preventDefault();
    var target = $(e.target);

    if (target.is('#navbarDropdownMenuLink')) {
        $('.dropdown-menu').toggleClass('show');
        $(this).toggleClass('show');
    }
    else{
        $('.dropdown-menu').removeClass('show');
        $(this).removeClass('show');
    }
});

$("#menu-toggle").click(function (e) {
    
    e.preventDefault();
    toggleSidebar();
});

function toggleSidebar(){

    $("#wrapper").toggleClass("toggled");
    $("#toggle-menu-icon").toggleClass("fa-times");
    $("#toggle-menu-icon").toggleClass("fa-bars");
}


