using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DogsVsCats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Esc検知用
        public bool _esc_flg = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            TrimScreen trimScreen = new TrimScreen();
            trimScreen.ShowDialog();//スクリーンショット処理開始
            _esc_flg = trimScreen._esc_flg;            

            //Escキーフラグ検知でQRReaderの処理を行わない
            if (!_esc_flg)
            {
                string folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var imageBytes = File.ReadAllBytes(folder + "\\TrimImage.jpg");
                DogVsCatClassfication.ModelInput sampleData = new DogVsCatClassfication.ModelInput()
                {
                    ImageSource = imageBytes,
                };

                //Load model and predict output
                var result = DogVsCatClassfication.Predict(sampleData);                            

            }
            else
            {
               
            }
        }

    }
}
