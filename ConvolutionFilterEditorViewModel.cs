using CG_Test.Filters;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace CG_Test
{
    public class FilterData
    {
        public string Name { get; set; }
        [JsonIgnore] // Prevents direct serialization of 2D array
        public double[,] Kernel { get; set; }
        [JsonPropertyName("Kernel")]
        public double[][] KernelSerializable
        {
            get => ConvertToJagged(Kernel);
            set => Kernel = ConvertTo2D(value);
        }
        public double Divisor { get; set; }
        public double Offset { get; set; }
        public Point Anchor { get; set; }
        private static double[][] ConvertToJagged(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            double[][] jaggedArray = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = array[i, j];
                }
            }
            return jaggedArray;
        }
        private static double[,] ConvertTo2D(double[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            double[,] array = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = jaggedArray[i][j];
                }
            }
            return array;
        }
    }
    public class ConvolutionFilterEditorViewModel : INotifyPropertyChanged
    {
        private readonly ConvolutionFilter originalFilter;
        public ObservableCollection<int> KernelSizes { get; set; }
        public ObservableCollection<KernelCell> KernelGrid { get; set; } = new();
        public ObservableCollection<string> AvailableFilters { get; set; } = new();
        private const string FilterFilePath = "filters.json";
        public string SelectedFilter { get; set; } = string.Empty;
        private void LoadFilters()
        {
            if (!File.Exists(FilterFilePath)) return;

            try
            {
                string json = File.ReadAllText(FilterFilePath);
                var filters = JsonSerializer.Deserialize<List<FilterData>>(json) ?? new List<FilterData>();
                AvailableFilters.Clear();
                foreach (var filter in filters)
                {
                    AvailableFilters.Add(filter.Name);
                }
            }
            catch (JsonException)
            {
                AvailableFilters.Clear();
            }
        }
        public void LoadFilter(string filterName)
        {
            if (!File.Exists(FilterFilePath)) return;

            try
            {
                string json = File.ReadAllText(FilterFilePath);
                var filters = JsonSerializer.Deserialize<List<FilterData>>(json);
                var selectedFilter = filters?.FirstOrDefault(f => f.Name == filterName);

                if (selectedFilter != null)
                {
                    SelectedFilter = filterName;
                    CustomFilter = new ConvolutionFilter
                    {
                        Kernel = selectedFilter.Kernel,
                        Divisor = selectedFilter.Divisor,
                        Offset = selectedFilter.Offset,
                        Anchor = selectedFilter.Anchor
                    };
                    OnPropertyChanged(nameof(CustomFilter));
                    OnPropertyChanged(nameof(SelectedKernelWidth));
                    OnPropertyChanged(nameof(SelectedKernelHeight));
                    OnPropertyChanged(nameof(SelectedFilter));
                    UpdateKernelGrid();
                }
            }
            catch (JsonException) { }
        }
        public void SaveFilter()
        {
            ApplyChangesToKernel();

            List<FilterData> filters = new();
            if (File.Exists(FilterFilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilterFilePath);
                    filters = JsonSerializer.Deserialize<List<FilterData>>(json) ?? new List<FilterData>();
                }
                catch (JsonException)
                {
                    filters = new List<FilterData>();
                }
            }

            filters.RemoveAll(f => f.Name == SelectedFilter);
            filters.Add(new FilterData
            {
                Name = SelectedFilter,
                Kernel = CustomFilter.Kernel,
                Divisor = CustomFilter.Divisor,
                Offset = CustomFilter.Offset,
                Anchor = CustomFilter.Anchor
            });

            try
            {
                string updatedJson = JsonSerializer.Serialize(filters, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilterFilePath, updatedJson);
                LoadFilters();
            }
            catch (IOException) { }
        }
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
            LoadFilters();
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
                    KernelGrid.Add(new KernelCell(i, j, value, i == CustomFilter.AnchorY && j == CustomFilter.AnchorX));
                }
            }
            OnPropertyChanged(nameof(KernelGrid));
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
        public void UpdateAnchor(int row, int col)
        {
            foreach (var cell in KernelGrid)
            {
                cell.IsAnchor = cell.Row == row && cell.Column == col;
                OnPropertyChanged(nameof(cell.IsAnchor));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
