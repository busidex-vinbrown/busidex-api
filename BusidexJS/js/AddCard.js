
$( document ).ready( function ()
{
    //$("div.ownerHelp").dialog({
    //    autoOpen:false,
    //    modal: true,
    //    height: 250,
    //    width: 400,
    //    resizable: false,
    //    title: 'Card Owner'
    //});

   
    //$("img.help").click(function() {
    //    $("div.ownerHelp").dialog('open');
    //});
    
    $( ".relCardView" ).click( function () 
    {
        $( this ).parent().find( ".relSelected" ).toggle();
        var cardId = $( this ).attr( "cardId" );
        var $hdnRelSelected = $( "#hdnRelSelected_" + cardId );
        if ( $hdnRelSelected.val() == "" )
        {
            $hdnRelSelected.val( cardId );
        } else
        {
            $hdnRelSelected.val( "" );
        }
    } );

    //var dialog = $( ".validation-summary-errors" ).dialog( {
    //    title: "Errors",
    //    resizable: false,
    //    modal: true,
    //    width: '400px',
    //    autoOpen: false
    //} );

    //dialog.dialog( 'open' );

    //$( "#imgFrontReset" ).attr( "src", $( "#frontPreview" ).attr( "src" ) );
    //$( "#imgBackReset" ).attr( "src", $( "#backPreview" ).attr( "src" ) );

    $( ".SizeWarning" ).click( function ()
    {
        $( "#fileSizeInfoMessage" ).show();
    } );

    $( ".ClosePopup" ).click( function ()
    {
        $( this ).parent().hide();
    } );

   $('.alert').hide();
    $(document).on("click", ".close", function () {
        $('.alert').hide();
    });

    /*$(document).on("click", "input.submitLink", function () {
        $(this).hide();
        if ($("#IsMyCard").is(":checked")) {// only required for card owner
        
            if (!$("#cardForm :input[name=IsMyCard]").is(":checked")) {
                $('.alert #errorMessage').html('Please indicate if you are the card owner or not.');
                $('.alert').show();
                $(this).show();
                return false;
            }
            
            if ($("#cardForm :input[name=Name]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter the name of the owner for this card.");
                $('.alert').show();
                $(this).show();
                return false;
            }
        
            if ($("#cardForm :input[name=Email]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter an email for this card.");
                $('.alert').show();
                $(this).show();
                return false;
            }

            if ($("#cardForm :input[name=Number]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter a phone number for this card.");
                $('.alert').show();
                $(this).show();
                return false;
            }
        }

        var hasFrontImage = $("#HasFrontImage").val();
        if ($("#cardForm :input[name=CardFrontImage]").val().length == 0 && hasFrontImage != "True" && $("#Display").val() == "IMG") {
            $('.alert #errorMessage').html("Please select an image to upload for this card.");
            $('.alert').show();
            $(this).show();
            return false;
        }

        try {            
            $("form#cardForm").submit();
        } catch (ex) {
            $('.alert #errorMessage').html(ex);
            $('.alert').show();
            $(this).show();
        }
        return true;
    } );*/
});