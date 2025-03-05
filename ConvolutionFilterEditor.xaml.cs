using CG_Test.Filters;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CG_Test
{
    public partial class ConvolutionFilterEditor : Window
    {
        private ConvolutionFilterEditorViewModel viewModel;
        public ConvolutionFilter? EditedFilter { get; private set; }
        public ConvolutionFilterEditor(ConvolutionFilter existingFilter)
        {
            InitializeComponent();
            viewModel = new(existingFilter);
            DataContext = viewModel;
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            viewModel.ApplyChangesToKernel();
            EditedFilter = viewModel.CustomFilter;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ResetFilter();
        }
    }
}