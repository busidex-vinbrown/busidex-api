
$( document ).ready( function ( ) {
    $( "input#AccountTypeId" ).attr( "disabled", "disabled" );
    $("input#AccountTypeId").eq(0).attr("checked", "checked").removeAttr("disabled");
    $("input#agree").removeAttr("checked");
    $( ".AccountType" ).eq( 0 ).css( "font-weight", "bold" );

    $( "input#UserName, input#Password, input#Email, input#Password, input#ConfirmPassword" ).keyup( function ( ) {
        if ( validateStep1() == false ) {
            $( "input.goToStep2" ).attr( "disabled", "disabled" );
        } else {
            $( "input.goToStep2" ).removeAttr( "disabled" );
        }
    } );
    $( "input#UserName, input#Password, input#Email, input#Password, input#ConfirmPassword" ).blur( function ( ) {
        if ( validateStep1() == false ) {
            $( "input.goToStep2" ).attr( "disabled", "disabled" );
        } else {
            $( "input.goToStep2" ).removeAttr( "disabled" );
        }
    } );
    
    $( "#step1img, #step2img, #step3img, #step4img" ).fadeTo( 'normal', .2 );

    $( "#step_1, #step_2, #step_3, #step_done" ).hide();

    //if ($("#cardOwnerToken").val() == '') { // $("#step1").show();
    switch ($("#StepHash").val()) {
        case 'step0':
            {
                step0();
                break;
            }
        case 'step1':
            {
                step1();
                break;
            }
        case 'step2':
            {
                step2();
                break;
            }
        case 'step3':
            {
                step3();
                break;
            }
        case 'step_done':
            {
                done();
                break;
            }
        default:
            {
                step1();
                break; 
            }
        }
    //}
    $( ".goToStep0" ).click( function () {
        step0();
    } );
    
    $( ".goToStep1" ).click( function ( ) {
        step1();
    } );

    $( ".goToStep2" ).click( function ( ) {
        step2();
    } );

    $( ".goToStep3" ).click( function ( ) {
        step3();
    } );

    $( "#agree" ).click( function () {
        
        if ( $( this ).is( ":checked" ) ) {
            $( "#register" ).removeAttr( "disabled" );
        } else {
            $( "#register" ).attr( "disabled", "disabled" );
        }
    } );

    $("#register").click(function () {
        $("#registerForm").submit();
    });
} );

function step0() {
    $(".registrationStep").hide();
    $("#step_0").show();
    $("#step1img, #step2img, #step3img").fadeTo('normal', .2);
    window.location.hash = "#step0";
}

function step1() {
    $(".registrationStep").hide();
    $("#step_1").show();
    $("#step1img").fadeTo('normal', 1);
    $("#step2img, #step3img").fadeTo('normal', .2);
    window.location.hash = "#step1";
}

function step2() {
    $(".registrationStep").hide();
    $("#step_2").show();
    $("#step1img,#step2img").fadeTo('normal', 1);
    $("#step3img").fadeTo('normal', .2);
    window.location.hash = "#step2";
}

function step3() {
    $(".registrationStep").hide();
    $("#step_3").show();
    $("#step1img,#step2img,#step3img").fadeTo('normal', 1);
    window.location.hash = "#step3";
}

function done() {
    $(".registrationStep").hide();
    $("#step_done").show();
    $("#step1img,#step2img,#step3img,#step4img").fadeTo('normal', 1);
    $("#instructionLabel").show();
    window.location.hash = "#step_done";
}

function validateStep1() {

    var userName = $( "input#UserName" ).val();
    var email = $( "input#Email" ).val();
    var confirmEmail = $( "input#ConfirmEmail" ).val();
    var password = $( "input#Password" ).val();
    var confirmPassword = $( "input#ConfirmPassword" ).val();

    var ok = $.trim(userName) != '' &&
        $.trim(email) != '' &&
        $.trim(password) != '' &&
        $.trim(confirmPassword) != '' &&
        password == confirmPassword &&
        email == confirmEmail;

    return ok;
}