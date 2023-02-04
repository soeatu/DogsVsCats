using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DogsVsCats
{
    /// <summary>
    /// TrimScreen.xaml の相互作用ロジック
    /// </summary>
    public partial class TrimScreen : Window
    {
        private Point _position;
        private bool _trimEnable = false;
        public bool ESC_Flg = false;

        public TrimScreen()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //　マルチモニターの全領域のサイズを取得
            this.Top = SystemParameters.VirtualScreenTop;
            this.Left = SystemParameters.VirtualScreenLeft;
            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;

            // ジオメトリサイズの設定
            this.ScreenArea.Geometry1 = new RectangleGeometry(new Rect(0.0, 0.0, this.Width, this.Height));        
        }
        /// <summary>
        /// Escキーでキャプチャ処理終了イベントメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //EscキーフラグをTure
                ESC_Flg = true;
                this.Close();
            }
        }
        private void Hand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null) { return; }
                
            // 開始座標を取得
            var point = e.GetPosition(path);
            _position = point;

            // マウスキャプチャの設定
            _trimEnable = true;
            this.Cursor = Cursors.Cross;
            path.CaptureMouse();
        }

        private void Hand_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null) { return; }                

            // 現在座標を取得
            var point = e.GetPosition(path);

            // マウスキャプチャの終了
            _trimEnable = false;
            this.Cursor = Cursors.Arrow;
            path.ReleaseMouseCapture();

            // 画面キャプチャ
            CaptureScreen(point);

            // アプリケーションの終了
            this.Close();
        }
        /// <summary>
        ///  //DrawingPathはパスを使って図形を書く MouseLeftButtonDownはこの要素の上にマウス ポインターがある状態でマウスの左ボタンが押されたときに発生します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null) { return; }               

            // 開始座標を取得
            var point = e.GetPosition(path);
            _position = point;

            // マウスキャプチャの設定
            _trimEnable = true;
            this.Cursor = Cursors.Cross;
            path.CaptureMouse();
        }

        /// <summary>
        /// DrawingPathはパスを使って図形を書く MouseLeftButtonDownはこの要素の上にマウス ポインターがある状態でマウスの左ボタンが離されたときに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null) { return; }                

            // 現在座標を取得
            var point = e.GetPosition(path);

            // マウスキャプチャの終了
            _trimEnable = false;
            this.Cursor = Cursors.Arrow;
            path.ReleaseMouseCapture();

            // 画面キャプチャ
            CaptureScreen(point);

            // アプリケーションの終了
            this.Close();
        }

        /// <summary>
        /// DrawingPathはパスを使って図形を書く MouseMoveはマウスがコントロール上を移動すると発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_trimEnable) { return; }

            var path = sender as Path;
            if (path == null) { return; }               

            // 現在座標を取得
            var point = e.GetPosition(path);

            // キャプチャ領域枠の描画
            DrawStroke(point);
        }

        /// <summary>
        /// 指定されている画面領域を描写
        /// </summary>
        /// <param name="point"></param>
        private void DrawStroke(Point point)
        {
            // 矩形の描画
            var x = _position.X < point.X ? _position.X : point.X;
            var y = _position.Y < point.Y ? _position.Y : point.Y;
            var width = Math.Abs(point.X - _position.X);
            var height = Math.Abs(point.Y - _position.Y);
            this.ScreenArea.Geometry2 = new RectangleGeometry(new Rect(x, y, width, height));
        }

        /// <summary>
        /// 実際に範囲指定された領域をキャプチャ　マウスの左ボタンを放されたときに実行
        /// </summary>
        /// <param name="point"></param>
        public void CaptureScreen(Point point)
        {
            // 座標変換
            var start = PointToScreen(_position);
            var end = PointToScreen(point);

            // キャプチャエリアの取得
            var x = start.X < end.X ? (int)start.X : (int)end.X;
            var y = start.Y < end.Y ? (int)start.Y : (int)end.Y;
            var width = (int)Math.Abs(end.X - start.X);
            var height = (int)Math.Abs(end.Y - start.Y);
            if (width == 0 || height == 0) { return; }

            // スクリーンイメージの取得
            using (var bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            using (var graph = System.Drawing.Graphics.FromImage(bmp))
            {
                // 画面をコピーする
                graph.CopyFromScreen(new System.Drawing.Point(x, y), new System.Drawing.Point(), bmp.Size);

                // イメージの保存
                string exeFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);               
                try
                {
                    bmp.Save(System.IO.Path.ChangeExtension(System.IO.Path.Combine(exeFolder, "Trimimage"), "jpg"), System.Drawing.Imaging.ImageFormat.Png);
                    bmp.Dispose();
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    MessageBox.Show("上手くスクリーンショットが行えませんでした。\nもう一度お試し下さい。", "警告", MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
        }
    }
}
