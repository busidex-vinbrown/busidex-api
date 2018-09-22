using System.Web;
using System.Web.Mvc;
using System.IO;
using Busidex.DAL;

namespace Busidex4.Controllers
{
    public class ImageResult : ActionResult
    {
        private Card _card;
        private bool _useBackImage;
        private bool _showDefault;
        private readonly System.Collections.Hashtable _mimeTypes = new System.Collections.Hashtable();

        public ImageResult( Card card, string back, bool showDefault)
        {
            Initiallize(card, back, showDefault);    
        }

        private void Initiallize(Card card, string back, bool showDefault)
        {
            _card = card;
            _useBackImage = back == "back";
            _showDefault = showDefault;

        }

        private void InitMimeTypes()
        {
            _mimeTypes.Add( "jpg", "image/jpeg" );
            _mimeTypes.Add( "jpeg", "image/jpeg" );
            _mimeTypes.Add( "png", "image/x-png" );
            _mimeTypes.Add( "gif", "image/gif" );
            _mimeTypes.Add( "bmp", "image/bmp" );
        }

        public override void ExecuteResult( ControllerContext context )
        {
            var response = context.RequestContext.HttpContext.Response;
            InitMimeTypes();
            response.Buffer = false;

            response.Clear();
            string name = string.IsNullOrEmpty( _card.Name ) ? "" : _card.Name;

            if ( _card != null )
            {
                if ( !_useBackImage )
                {
                    response.ContentType = _mimeTypes[ _card.FrontType.ToLower().Trim() ].ToString();
                    //response.Headers[ "Content-Disposition" ] = "attachment; filename=" + name.Replace( " ", "_" ) + "." +
                    //                                            _card.FrontType;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_card.BackType))
                    {
                        response.ContentType = _mimeTypes[ _card.BackType.ToLower().Trim() ].ToString();
                        //response.Headers[ "Content-Disposition" ] = "attachment; filename=" + name.Replace( " ", "_" ) +
                        //                                            "." +
                        //                                            _card.BackType;
                    }
                }



                if ( _useBackImage == false )
                {
                    if ( _card.FrontImage != null )
                    {
                        response.BinaryWrite( _card.FrontImage.ToArray() );
                    }

                }
                else
                {
                    if (_card.BackImage != null) {
                        response.BinaryWrite(_card.BackImage.ToArray());
                        response.BufferOutput = true;
                    } else if (_showDefault)
                    {
                        HttpServerUtilityBase server = context.HttpContext.Server;
                        var fi = new FileInfo ( server.MapPath ( "~\\Images\\NoneChosen.png" ) );
                        var imageData = new byte[fi.Length];
                        using ( FileStream fs = fi.OpenRead() )
                        {
                            fs.Read ( imageData, 0, ( int ) fi.Length );
                            response.BinaryWrite ( imageData );
                        }
                    }
                }
            }
            

            if ( response.IsClientConnected )
            {
                response.Flush();
                response.End();
            }

        }

     
    }
}