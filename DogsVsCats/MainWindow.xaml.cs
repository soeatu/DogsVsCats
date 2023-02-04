using System.IO;
using System.Reflection;
using System.Windows;

namespace DogsVsCats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Esc検知用
        public bool Esc_flg = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            TrimScreen trimScreen = new TrimScreen();
            trimScreen.ShowDialog();//スクリーンショット処理開始
            Esc_flg = trimScreen.ESC_Flg;            

            //Escキーフラグ検知で処理を行わない
            if (!Esc_flg)
            {
                string folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var imageBytes = File.ReadAllBytes(folder + "\\TrimImage.jpg");
                DogVsCatClassfication.ModelInput sampleData = new DogVsCatClassfication.ModelInput()
                {
                    ImageSource = imageBytes,
                };

                //Load model and predict output
                var result = DogVsCatClassfication.Predict(sampleData);

                //判別結果　表示
                ResultTextBox.Text = result.PredictedLabel;
            }        
        }     
    }
}
