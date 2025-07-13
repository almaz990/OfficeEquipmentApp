using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using OfficeEquipment;
using System.Collections.Generic;
using System.ComponentModel;


namespace OfficeEquipment
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Equipment> _equipmentList = new(); // Список оборудования
        private int _currentId = 1; // Начальный ID первого элемента в списке оборудования
        private Equipment _currentEquipment = null; // Текущий элемент

        public MainWindow()
        {
            InitializeComponent(); // Загрузка начального окна
            LoadData(); // Добавление начальных данных в окно

            EquipmentGrid.ItemsSource = _equipmentList; // Обновление списка оборудования
            UpdateRecordCount();
        }

        private void LoadData()
        {
            var data = XmlDataService.LoadData();
            _equipmentList = new ObservableCollection<Equipment>(data);

            // Находим максимальный ID для продолжения нумерации
            _currentId = _equipmentList.Any() ? _equipmentList.Max(e => e.Id) + 1 : 1;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            SaveData();
        }

        private void SaveData()
        {
            XmlDataService.SaveData(_equipmentList.ToList());
        }

        // Обновление количества записей
        private void UpdateRecordCount()
        {
            RecordCountText.Text = _equipmentList.Count.ToString();
            StatusText.Text = $"Записей: {_equipmentList.Count} | Выбрано: {EquipmentGrid.SelectedItems.Count}";
        }

        // Открыть окно добавления
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _currentEquipment = new Equipment { Id = _currentId };
            IdTextBlock.Text = "Новый";
            NameTextBox.Text = "";
            TypeComboBox.SelectedIndex = 0;
            StatusComboBox.SelectedIndex = 0;
            EditForm.Visibility = Visibility.Visible; // Включаем форму изменения(добавления)
        }
        // Изменение записи в списке
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на то, что какая-то запись была выбрана
            if (EquipmentGrid.SelectedItem is Equipment selected)
            {
                _currentEquipment = selected;
                IdTextBlock.Text = selected.Id.ToString();
                NameTextBox.Text = selected.Name;
                TypeComboBox.SelectedItem = selected.Type;
                StatusComboBox.SelectedItem = selected.Status;

                EditForm.Visibility = Visibility.Visible; // Включаем форму изменения(добавления)
            }
            else
            {
                MessageBox.Show("Выберите оборудование для редактирования", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Удаление какой-либо записи из списка
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на то, что какая-то запись была выбрана
            if (EquipmentGrid.SelectedItem is Equipment selected)
            {
                var result = MessageBox.Show($"Удалить оборудование '{selected.Name}'?", "Подтверждение удаления",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _equipmentList.Remove(selected);
                    UpdateRecordCount();
                }
            }
            else
            {
                MessageBox.Show("Выберите оборудование для удаления", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Сохранение редактирования или добавления
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на то, что в поле наименования не пусто
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите наименование оборудования", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Присваивание значений полям нового(измененного) объекта
            _currentEquipment.Name = NameTextBox.Text;
            _currentEquipment.Type = TypeComboBox.SelectedItem.ToString();
            _currentEquipment.Status = StatusComboBox.SelectedItem.ToString();

            // Присвоение уникального ID при добавлении
            if (_currentEquipment.Id == _currentId)
            {
                _currentEquipment.Id = _currentId++;
                _equipmentList.Add(_currentEquipment);
            }


            EquipmentGrid.Items.Refresh(); // Обновление таблицы записей 
            EditForm.Visibility = Visibility.Collapsed; // Выключаем форму изменения
            UpdateRecordCount(); // Обновляем статус
        }
        // Отмена сохранения или редактирования
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditForm.Visibility = Visibility.Collapsed; // Выключаем форму изменения
        }
        // Применить фильтр
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Считывание значений полей типа и статуса
            string typeFilter = FilterTypeComboBox.SelectedItem?.ToString();
            string statusFilter = FilterStatusComboBox.SelectedItem?.ToString();

            if (typeFilter == "Все типы" && statusFilter == "Все статусы")
            {
                EquipmentGrid.ItemsSource = _equipmentList; // Отображение всех наименований
            }
            else
            {
                // LINQ запрос для создания нового отфильтрованного списка
                var filtered = _equipmentList.Where(eq =>
                    (typeFilter == "Все типы" || eq.Type == typeFilter) &&
                    (statusFilter == "Все статусы" || eq.Status == statusFilter));

                // Новый список
                EquipmentGrid.ItemsSource = new ObservableCollection<Equipment>(filtered);
            }

            UpdateRecordCount();
        }
        // Отображение количества выбранных элементов
        private void EquipmentGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateRecordCount();
        }
    }
}