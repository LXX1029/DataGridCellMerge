using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1
{
    /// <summary>
    /// DataGridMergerCell.xaml 的交互逻辑
    /// </summary>
    [INotifyPropertyChanged]
    public partial class DataGridMergerCell : Window
    {
        public DataGridMergerCell()
        {
            InitializeComponent();
            this.Loaded += DataGridMergerCell_Loaded;
        }

        private List<EquipmentInfo> EquipmentInfosList = new();
        private async void DataGridMergerCell_Loaded(object sender, RoutedEventArgs e)
        {
            // 模拟数据
            var list = new EquipmentInfo[]
            {
                new EquipmentInfo{ DepartmentName="生产部", EquipmentType="加工设备", EquipmentName="车床", EquipmentStatus="正常" },
                new EquipmentInfo{ DepartmentName="生产部", EquipmentType="加工设备", EquipmentName="钻床", EquipmentStatus="正常" },
                new EquipmentInfo{ DepartmentName="加工部", EquipmentType="切割设备", EquipmentName="激光切割机", EquipmentStatus="异常" },
                new EquipmentInfo{ DepartmentName="加工部", EquipmentType="切割设备", EquipmentName="水刀切割机", EquipmentStatus="异常" },
                new EquipmentInfo{ DepartmentName="维修部", EquipmentType="焊接设备", EquipmentName="MIG焊机", EquipmentStatus="正常" },
                new EquipmentInfo{ DepartmentName="维修部", EquipmentType="焊接设备", EquipmentName="TIG焊机", EquipmentStatus="正常" },
                new EquipmentInfo{ DepartmentName="维修部", EquipmentType="焊接设备", EquipmentName="电弧焊机", EquipmentStatus="异常" },
            };
            this.EquipmentInfosList = list.OrderBy(x => x.DepartmentName).ToList();
            var view = CollectionViewSource.GetDefaultView(EquipmentInfosList);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("DepartmentName"));
            await Task.Delay(500);
            this.datagrid.ItemsSource = view.Groups;
            view.Refresh();
        }

        [ObservableProperty]
        private ObservableCollection<EquipmentInfo> equipmentInfosCollection = new();

        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(EquipmentInfosList);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("DepartmentName"));
            this.datagrid.ItemsSource = view.Groups;
            view.Refresh();
        }
    }

    /// <summary>
    /// 数据类
    /// </summary>
    public class EquipmentInfo
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentType { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string EquipmentStatus { get; set; }
        public bool Equals(EquipmentInfo? other)
        {
            if (other == null) return false;
            return this.DepartmentName == other.DepartmentName && this.EquipmentName == other.EquipmentName && this.EquipmentType == other.EquipmentType;
        }
    }

    /// <summary>
    /// 单元格高度转换类
    /// </summary>
    public class DataGridCellHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int height = 40;
            int finalHeight = 40;
            if (value is null) return height;
            if (value is CollectionViewGroup viewGroup)
            {
                var itemsCount = viewGroup.Items.Count;
                finalHeight = height * itemsCount;
            }
            return finalHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }


    /// <summary>
    /// 单元格数据源转换类
    /// </summary>
    public class DataGridCellDataSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null) return Enumerable.Empty<object>();
            var readOnlyCollection = value as ReadOnlyObservableCollection<object>;
            if (readOnlyCollection == null) return Enumerable.Empty<object>();
            if (int.TryParse(parameter.ToString(), out int displayIndex))
            {
                ObservableCollection<EquipmentInfo> collection = new();
                foreach (var item in readOnlyCollection)
                {
                    collection.Add((EquipmentInfo)item);
                }
                var view = CollectionViewSource.GetDefaultView(collection);
                if (displayIndex == 1)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("EquipmentType"));
                    return view.Groups;
                }
                if (displayIndex == 2)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("EquipmentName"));
                    return view.Groups;
                }
                if (displayIndex == 3)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("EquipmentStatus"));
                    return collection;
                }
                return collection;
            }
            return Enumerable.Empty<object>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
