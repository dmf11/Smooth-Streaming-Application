//smooth streaming SDK: 
//http://visualstudiogallery.msdn.microsoft.com/04423d13-3b3e-4741-a01c-1ae29e84fea6/view/Discussions/2

using Windows.Media;
using Microsoft.Media.AdaptiveStreaming;
using Windows.UI.Core;

using System.IO;
using System.Text;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.Serialization;

using System.Linq;
using Windows.Foundation;   
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StreamingProtoType
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page    
    {

        private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                       
        private MediaExtensionManager extensions = new MediaExtensionManager();

        private Windows.Foundation.Collections.PropertySet propertySet = new Windows.Foundation.Collections.PropertySet();
        private IAdaptiveSourceManager adaptiveSourceManager;

        private AdaptiveSource adaptiveSource = null;

        private AdaptiveSourceStatusUpdatedEventArgs adaptiveSourceStatusUpdate;

        private Manifest manifestObject;

        //Responsible for processing the window messages and dispatching events to the client.
        public static CoreDispatcher _dispatcher;
        private DispatcherTimer sliderPositionUpdateDispatcher;       

        public MainPage()
        {
            this.InitializeComponent();


            // Gets the default instance of AdaptiveSourceManager which manages Smooth Streaming media sources.
            adaptiveSourceManager = AdaptiveSourceManager.GetDefault();
            
            //Property sets are used to indicate what properties of an item or folder should be loaded when binding to an existing item or folder or when loading an item or folder's properties.
            // Sets property key value to AdaptiveSourceManager default instance.
            // {A5CE1DE8-1D00-427B-ACEF-FB9A3C93DE2D}" must be hardcoded.
            propertySet["{A5CE1DE8-1D00-427B-ACEF-FB9A3C93DE2D}"] = adaptiveSourceManager;
            //The MediaElement control does not support Smooth Streaming content out-of-box. To enable the Smooth Streaming support,
            //you must register the Smooth Streaming byte-stream handler by file name extension and MIME type. To register, you use the 
            //MediaExtensionManager.RegisterByteStremHandler method of the Windows.Media namespace.


            // Registers Smooth Streaming byte-stream handler for ".ism" extension and, 
            // "text/xml" and "application/vnd.ms-ss" mime-types and pass the propertyset. 
            // http://*.ism/manifest URI resources will be resolved by Byte-stream handler.
            extensions.RegisterByteStreamHandler(
                "Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler",
                ".ism",
                "text/xml",
                propertySet);
            extensions.RegisterByteStreamHandler(
                "Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler",
                ".ism",
                "application/vnd.ms-sstr+xml",
                propertySet);
            //The source resolver takes a URL or byte stream and creates the appropriate media source for that content.
            //The source resolver is the standard way for the applications to create media sources. 

           
            //subscribes to the adaptive source open event
            adaptiveSourceManager.AdaptiveSourceOpenedEvent += new AdaptiveSourceOpenedEventHandler(mediaElement_AdaptiveSourceOpened); 
            //adaptiveSourceManager.AdaptiveSourceStatusUpdated += new AdaptiveSourceClosedEventHandler(mediaElement_AdaptiveSourceStatusUpdated);
            adaptiveSourceManager.AdaptiveSourceStatusUpdatedEvent += new AdaptiveSourceStatusUpdatedEventHandler(mediaElement_AdaptiveSourceStatusUpdated);
            adaptiveSourceManager.AdaptiveSourceStatusUpdatedEvent += adaptiveSourceManager_AdaptiveSourceStatusUpdatedEvent;
            //adaptiveSourceManager.AdaptiveSourceClosedEvent += new AdaptiveSourceClosedEventArgs(mediaElement_AdaptiveSourceClosed);

            //required to subscribe to the media element events
            mediaElement.MediaOpened += MediaOpened;
            mediaElement.MediaEnded += MediaEnded;
            mediaElement.MediaFailed += MediaFailed;

            //code for the media player slider
            _dispatcher = Window.Current.Dispatcher;
            PointerEventHandler pointerpressedhandler = new PointerEventHandler(sliderProgress_PointerPressed);
            //sliderProgress.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);


        }

        void timer_Tick(object sender, object e)
        {

      
        }

        void adaptiveSourceManager_AdaptiveSourceStatusUpdatedEvent(AdaptiveSource sender, AdaptiveSourceStatusUpdatedEventArgs args)
        {
            //throw new NotImplementedException();
            try
            {
                //Schedules the provided callback on the UI thread from a worker thread, and returns the results asynchronously.
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {                  
                    //log(" AdaptiveSource Opened: " + "URI: " + displayedUri);
                   // log(args.UpdateType + " " + additionalInfo); //removed args.startTime() and args.EndTime()
                   // log(stopWatch.Elapsed + "  " + args.UpdateType + args.AdditionalInfo + "\n");
                    log("Time: " + stopWatch.Elapsed + "  " + args.UpdateType + "  " + args.AdditionalInfo + "\n\n");
                            
                });
                
                }
            catch(Exception ex)
            {
                //Schedules the provided callback on the UI thread from a worker thread, and returns the results asynchronously.
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    log("error" + ex.Message.ToString());
                });
               
            }

        }//End of MainPage()       

        
        //logging the outputted data into the text box.
        public void log(string line) 
        {
            txtBandwidth.Text += line;
        }//end of log() method

        #region This code was taken from the following tutorial : http://azure.microsoft.com/en-us/documentation/articles/media-services-build-smooth-streaming-apps/
        #region UI Button Click Events
        private void BtnSetSource_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Start();
           // mediaElement.Source = new Uri("http://ecn.channel9.msdn.com/o9/content/smf/smoothcontent/elephantsdream/Elephants_Dream_1024-h264-st-aac.ism/manifest");           
            mediaElement.Source = new Uri("http://www.ssvideo.ie/video/ElephantsDream.ism/manifest");
        
        }


        private void sliderProgress_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //mediaElement.Position = new TimeSpan(0, 0, (int)(sliderProgress.Value));
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {            
            mediaElement.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
        }
        #endregion This code was taken from the following tutorial : http://azure.microsoft.com/en-us/documentation/articles/media-services-build-smooth-streaming-apps/

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           
        }
        #endregion UI Button Click Events

        #region Adaptive Source Manager Level Events
        private void mediaElement_AdaptiveSourceOpened(AdaptiveSource sender, AdaptiveSourceOpenedEventArgs args)
        {
            adaptiveSource = args.AdaptiveSource;
        }
        #endregion Adaptice source manager level events

        #region Adaptive Source Level Events
        private void mediaElement_ManifestReady(AdaptiveSource sender, ManifestReadyEventArgs args)
        {
            adaptiveSource = args.AdaptiveSource;
            manifestObject = args.AdaptiveSource.Manifest;
        }
        private void mediaElement_AdaptiveSourceStatusUpdated(AdaptiveSource sender, AdaptiveSourceStatusUpdatedEventArgs args)
        {
            adaptiveSourceStatusUpdate = args;
        }
        private void mediaElement_AdaptiveSourceFailed(AdaptiveSource sender, AdaptiveSourceFailedEventArgs args)
        {
            MessageDialog msDialog = new MessageDialog("An Error Occured: " + args.HttpResponse, "Error!");
        }
        #endregion Adaptive Source Level Events

        #region Media Element Event Handlers
        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            MessageDialog msDialog = new MessageDialog("MediaElement Opened", "Media Opened!");
        }
        private void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageDialog msDialog = new MessageDialog("An Error Occured:" + e.ErrorMessage, "Error!");
        }
        private void MediaEnded(object sender, RoutedEventArgs e)
        {
            MessageDialog msDialog = new MessageDialog("Media Element has Ended", "Media Ended!");
        }
        #endregion Media Element Event Handlers

        private void sliderProgress_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void txtStatus_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    
       
        //public delegate void AdaptiveSourceOpenedEventHandler(AdaptiveSource adaptiveSource,AdaptiveSourceOpenedEventArgs e);   
        public delegate void AdaptiveSourceClosedEventHandler(AdaptiveSource adaptiveSource ,AdaptiveSourceClosedEventArgs e);
        public delegate void ManifestReadyEventHandler(AdaptiveSource adaptiveSource, ManifestReadyEventArgs e);   
                   

        }//end of public sealed partial class

    }//end of class

