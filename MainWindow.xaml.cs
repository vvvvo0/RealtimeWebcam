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


        // 각 필터의 적용 여부를 저장하는 bool 타입 변수 선언
        private bool isGrayscale; // 흑백 필터 적용 여부
        private bool isBlur; // 블러 필터 적용 여부
        private bool isInvert; // 색상 반전 필터 적용 여부

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


            isGrayscale = false;
            isBlur = false;
            isInvert = false;
        }


        private void UpdateFrame(object? sender, EventArgs e)
        {
            capture.Read(frame); // 웹캠에서 프레임을 읽어와 frame 객체에 저장

            if (!frame.Empty()) // frame 객체가 비어 있지 않으면(프레임을 읽어오는 데 성공하면),
            {
                Mat filteredFrame = frame.Clone(); // 원본 프레임 복사
                                                   // (필터를 적용하기 전에 원본 프레임을 복사하여
                                                   // 필터가 원본 프레임에 영향을 주지 않도록 한다.)


                // isGrayscale, isBlur, isInvert 변수 값에 따라 해당 필터를 적용한다.
                if (isGrayscale) // 흑백 필터 적용
                {
                    Cv2.CvtColor(filteredFrame, filteredFrame, ColorConversionCodes.BGR2GRAY);
                }


                if (isBlur) // 블러 필터 적용
                {
                    Cv2.Blur(filteredFrame, filteredFrame, new OpenCvSharp.Size(5, 5));
                }


                if (isInvert) // 색상 반전 필터 적용
                {
                    Cv2.BitwiseNot(filteredFrame, filteredFrame);
                }


                // 필터링된 이미지를 WPF 컨트롤에 표시
                imgDisplay.Source = WriteableBitmapConverter.ToWriteableBitmap(filteredFrame);


                filteredFrame.Dispose();
            }


        }



        // GrayscaleButton_Click(), BlurButton_Click(), InvertButton_Click() 메서드:
        // 각 필터 버튼의 Click 이벤트 핸들러로, 해당 필터의 적용 여부를 토글한다.
        private void GrayscaleButton_Click(object sender, RoutedEventArgs e)
        {
            isGrayscale = !isGrayscale; // 흑백 필터 토글
        }


        //  블러 필터를 적용
        private void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            isBlur = !isBlur; // 블러 필터 토글
        }


        // 색상 반전 필터를 적용
        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            isInvert = !isInvert; // 색상 반전 필터 토글
        }
    }
}