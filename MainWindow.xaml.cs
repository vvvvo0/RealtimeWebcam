using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace RealtimeWebcam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private VideoCapture capture; // 웹캠 캡처를 위한 VideoCapture 객체
        private Mat frame; // 이미지 프레임을 저장하는 Mat 객체
        private DispatcherTimer timer; // UI 스레드에서 일정 간격으로 작업을 수행하기 위한 DispatcherTimer 객체



        public MainWindow()
        {
            InitializeComponent();

            capture = new VideoCapture(0); // 기본 웹캠 장치를 사용하여 VideoCapture 객체를 초기화
            frame = new Mat(); // Mat 객체를 생성
            timer = new DispatcherTimer(); // DispatcherTimer 객체를 생성
            timer.Interval = TimeSpan.FromMilliseconds(33); // 타이머 간격을 33밀리초로 설정하여,
                                                            // 약 30fps로 프레임을 업데이트한다
            timer.Tick += UpdateFrame; // timer의 Tick 이벤트에 UpdateFrame 메서드를 연결
            timer.Start(); // 타이머를 시작한다
        }


        private void UpdateFrame(object sender, EventArgs e)
        {
            capture.Read(frame); // 웹캠에서 프레임을 읽어와 frame 객체에 저장
            if (!frame.Empty()) // frame 객체가 비어 있지 않으면, 즉 프레임을 읽어오는 데 성공하면 다음 코드를 실행한다.
            {
                imgDisplay.Source = frame.ToBitmapSource(); // frame 객체를 BitmapSource로 변환하여,
                                                            // imgDisplay 컨트롤의 Source 프로퍼티에 할당한다.
            }
        }

        // 흑백 필터 기능 구현
        private void GrayscaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!frame.Empty())
            {
                Mat grayFrame = new Mat();
                Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);
                imgDisplay.Source = grayFrame.ToBitmapSource();
                grayFrame.Dispose();
            }
        }


        //  블러 필터를 적용
        private void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            Mat blurFrame = new Mat();
            Cv2.Blur(frame, blurFrame, new OpenCvSharp.Size(5, 5));
            imgDisplay.Source = WriteableBitmapConverter.ToWriteableBitmap(blurFrame);
            blurFrame.Dispose();
        }


        // 색상 반전 필터를 적용
        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            Mat invertFrame = new Mat();
            Cv2.BitwiseNot(frame, invertFrame);
            imgDisplay.Source = WriteableBitmapConverter.ToWriteableBitmap(invertFrame);
            invertFrame.Dispose();
        }
    }
}