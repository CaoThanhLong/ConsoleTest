using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using Filter;

namespace CoreFunctionTest
{
    class ConsoleDemo
    {
        static async Task Main(string[] args)
        {
            AudioTrackSource microphoneSource = null;
            VideoTrackSource webcamSource = null;
            Transceiver audioTransceiver = null;
            Transceiver videoTransceiver = null;
            LocalAudioTrack localAudioTrack = null;
            LocalVideoTrack localVideoTrack = null;

            
            try
            {
                var deviceList = await DeviceVideoTrackSource.GetCaptureDevicesAsync();

                foreach (var device in deviceList)
                {
                    Console.WriteLine($"Found webcam {device.name} (id: {device.id})");
                }

                using var peerConnection = new PeerConnection();
                var config = new PeerConnectionConfiguration
                {
                    IceServers = new List<IceServer>
                {
                    new IceServer{ Urls = { "stun:stun.l.google.com:19302" } }
                }
                };

                await peerConnection.InitializeAsync(config);
                Console.WriteLine("Peer connection initialized.");

                // Video
                webcamSource = await DeviceVideoTrackSource.CreateAsync();
                var videoTrackConfig = new LocalVideoTrackInitConfig
                {
                    trackName = "webcam_track"
                };
                localVideoTrack = LocalVideoTrack.CreateFromSource(webcamSource, videoTrackConfig);

                videoTransceiver = peerConnection.AddTransceiver(MediaKind.Video);
                videoTransceiver.LocalVideoTrack = localVideoTrack;
                videoTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;


                videoTransceiver.LocalVideoTrack.Argb32VideoFrameReady += LocalVideoTrack_Argb32VideoFrameReady;

                // Audio
                microphoneSource = await DeviceAudioTrackSource.CreateAsync();
                var audioTrackConfig = new LocalAudioTrackInitConfig
                {
                    trackName = "microphone_track"
                };
                localAudioTrack = LocalAudioTrack.CreateFromSource(microphoneSource, audioTrackConfig);


                
                audioTransceiver = peerConnection.AddTransceiver(MediaKind.Audio);
                audioTransceiver.LocalAudioTrack = localAudioTrack;
                audioTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

            //localAudioTrack?.Dispose();
            //localVideoTrack?.Dispose();
            //microphoneSource?.Dispose();
            //webcamSource?.Dispose();
        }

        private static void LocalVideoTrack_Argb32VideoFrameReady(Argb32VideoFrame frame)
        {
            Console.WriteLine("Here");

        }
    }
}
