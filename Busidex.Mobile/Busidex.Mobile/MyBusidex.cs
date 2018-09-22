
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Json;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;
using PointF = Android.Graphics.PointF;

namespace Busidex.Mobile
{
	[Activity (Label = "MyBusidex", MainLauncher = false)]			
	public class MyBusidex : Activity
	{
	    protected override void OnCreate(Bundle bundle)
	    {
	        base.OnCreate(bundle);

	        // Create your application here
	        SetContentView(Resource.Layout.MyBusidex);

	        Button btnBackAddCard = FindViewById<Button>(Resource.Id.btnBackAddCard);

	        btnBackAddCard.Click += delegate
	                                    {
	                                        var aAddCard = new Intent(this, typeof (AddCard));
	                                        StartActivity(aAddCard);
	                                    };

	        try
	        {
	            HttpWebResponse response = GetWebResponse("http://busidex.com/api/Busidex/MyBusidex?id=65");

	            if (response != null)
	            {
	                using (var sr = new StreamReader(response.GetResponseStream()))
	                {
                        
	                    var result = sr.ReadToEnd();
	                    var images = JsonArray.Parse(result);

	                    foreach (var image in images)
	                    {
	                        string thisimage = image.ToString();
                            //var webClient = new WebClient();
                            //byte[] imageBytes = webClient.DownloadData(thisimage);
                            //var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);


	                    }
	                    //                in = new InputStream(new URL(url).openStream(), IO_BUFFER_SIZE);

	                    //                ByteArrayOutputStream dataStream = new ByteArrayOutputStream();
	                    //out = new OutputStream(dataStream, IO_BUFFER_SIZE);
	                    //copy(in, out);
	                    //out.flush();

	                    //        byte[] data = dataStream.toByteArray();
	                    //BitmapFactory.Options options = new BitmapFactory.Options();
	                    ////options.inSampleSize = 1;

	                    //bitmap = BitmapFactory.DecodeByteArray(data, 0, data.length,options);
	                }
	            }
	        }
	        catch (Exception ex)
	        {
	            Console.WriteLine(ex.Message);
	        }
	    }

	    private HttpWebResponse GetWebResponse(string url)
		{
			//string authInfo = username + ":" + password;
			//authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			//request.Headers["Authorization"] = "Basic " + authInfo;
			
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch(System.Net.WebException webException)
			{
				Console.WriteLine("ERROR getting response stream from: {0}", url);
				Console.Write(webException.Message);
			}
			return response;
		}
	}
}

