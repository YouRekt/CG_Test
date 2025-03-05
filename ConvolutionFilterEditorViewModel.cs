using CG_Test.Filters;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CG_Test
{
    public class ConvolutionFilterEditorViewModel : INotifyPropertyChanged
    {
        private readonly ConvolutionFilter originalFilter;
        public ObservableCollection<int> KernelSizes { get; set; }
        public ObservableCollection<KernelCell> KernelGrid { get; set; } = new();
        private bool _autoCalculateDivisor;
        public bool AutoCalculateDivisor
        {
            get => _autoCalculateDivisor;
            set
            {
                if (_autoCalculateDivisor != value)
                {
                    _autoCalculateDivisor = value;
                    OnPropertyChanged(nameof(AutoCalculateDivisor));
                    OnPropertyChanged(nameof(IsDivisorEnabled));
                    if (_autoCalculateDivisor)
                    {
                        UpdateDivisor();
                    }
                }
            }
        }

        // This property is used to enable/disable the divisor TextBox.
        public bool IsDivisorEnabled => !AutoCalculateDivisor;
        public int SelectedKernelWidth
        {
            get => CustomFilter.KernelWidth;
            set
            {
                CustomFilter.KernelWidth = value;
                OnPropertyChanged(nameof(SelectedKernelWidth));
                UpdateKernelGrid();
            }
        }
        public int SelectedKernelHeight
        {
            get => CustomFilter.KernelHeight;
            set
            {
                CustomFilter.KernelHeight = value;
                OnPropertyChanged(nameof(SelectedKernelHeight));
                UpdateKernelGrid();
            }
        }
        public ConvolutionFilter CustomFilter { get; set; }
        public ConvolutionFilterEditorViewModel(ConvolutionFilter filter)
        {
            originalFilter = filter;
            CustomFilter = filter.Clone();
            KernelSizes = new ObservableCollection<int> { 1, 3, 5, 7, 9 };
            AutoCalculateDivisor = false;
            UpdateKernelGrid();
        }
        private void UpdateKernelGrid()
        {
            KernelGrid.Clear();

            for (int i = 0; i < SelectedKernelHeight; i++)
            {
                for (int j = 0; j < SelectedKernelWidth; j++)
                {
                    double value = (i < CustomFilter.KernelHeight && j < CustomFilter.KernelWidth)
                        ? CustomFilter.Kernel[i, j]
                        : 0;
                    KernelGrid.Add(new KernelCell(i, j, value));
                }
            }
        }
        public void ApplyChangesToKernel()
        {
            for (int i = 0; i < SelectedKernelHeight; i++)
            {
                for (int j = 0; j < SelectedKernelWidth; j++)
                {
                    CustomFilter.Kernel[i, j] = KernelGrid[i * SelectedKernelWidth + j].Value;
                }
            }
            if (AutoCalculateDivisor)
            {
                UpdateDivisor();
            }
        }
        public void ResetFilter()
        {
            CustomFilter = originalFilter.Clone();
            OnPropertyChanged(nameof(CustomFilter));
            OnPropertyChanged(nameof(SelectedKernelWidth));
            OnPropertyChanged(nameof(SelectedKernelHeight));
            UpdateKernelGrid();
            if (AutoCalculateDivisor)
            {
                UpdateDivisor();
            }
        }
        private void UpdateDivisor()
        {
            double sum = 0;
            // Calculate the sum from the working copy's kernel.
            for (int i = 0; i < SelectedKernelHeight; i++)
            {
                for (int j = 0; j < SelectedKernelWidth; j++)
                {
                    sum += CustomFilter.Kernel[i, j];
                }
            }
            CustomFilter.Divisor = (sum == 0) ? 1 : sum;
            OnPropertyChanged(nameof(CustomFilter));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
