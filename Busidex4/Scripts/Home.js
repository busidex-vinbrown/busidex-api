$(document).ready(function() {

    //$(document).on("click", "#login", function () {
       
    //});
    
    if ($(".feature").length > 0) {
        $("ul#main-menu li").hide();
    }

    var backgroundColor = $("div.feature").css('background-color');

    $("div.feature, ol.round li").hover(
        function() {
            $(this).css({ backgroundColor: backgroundColor }).stop().animate({ backgroundColor: "#ddd" }, 'slow');
        },
        function() {
            $(this).stop().animate({ backgroundColor: backgroundColor }, 'slow');
        }
    );

    $('.carousel').carousel({
        interval: 10000,
    }).bind('slid', function() {
        $('.active').find('.carousel-subtext').eq(0).fadeIn();
        $('.active').find('.carousel-caption').eq(0).fadeIn();
    }).bind('slide', function () {
        $('.active').find('.carousel-subtext').eq(0).fadeOut();
        $('.active').find('.carousel-caption').eq(0).fadeOut();
    });
    
    $('.active').find('.carousel-subtext').eq(0).fadeIn();
    $('.active').find('.carousel-caption').eq(0).fadeIn();
});