using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
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
using CSharpToBlockly;

namespace CSharpBlock.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string code = @"namespace EllabitChallenge { class Challenge { void Invoke() { dynamic n = ""1"";for(int intcount=0;count<4;count++){n=(n*2);}}}}";
            //string code1 = "namespace EllabitChallenge { class Challenge { void Invoke() { dynamic n;n=1;for(int intcount=0;count<4;count++){n=(n*2);}}}}";
            //string code2 = "{dynamic n;n=1;for(int intcount=0;count<4;count++){n=(n*2);}}";

            //SyntaxTree codeTree = CSharpSyntaxTree.ParseText(code ?? "");
            //var node = codeTree.GetRoot();
            txtCode.Text = SharpParse.Parse(code).ToString();
        }
    }
}
