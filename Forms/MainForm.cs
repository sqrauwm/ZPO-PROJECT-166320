using System;
using System.Linq;
using System.Windows.Forms;
using WardrobeApp.Data;
using WardrobeApp.Exceptions;
using WardrobeApp.Models;
using System.Drawing;

namespace WardrobeApp.Forms
{
    public class MainForm : Form
    {
        private const int WardrobeCapacity = 30;
        private DataManager _dataManager;

        private DataGridView dataGridView1;
        private ProgressBar progressBar1;
        private Label lblCapacity1;
        private TextBox txtName1;
        private ComboBox txtCategory1;
        private TextBox txtColor1;
        private TextBox txtSize1;
        private TextBox txtMaterial1;
        private CheckBox checkFavorite1;
        private TextBox txtSearch1;
        private ComboBox ComboFilterCategory1;
        private Button btnSearch1;
        private Button btnFilter1;
        private Button btnAdd1;
        private Button btnRemove1;
        private Button btnSave1;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colColor;
        private Label title1;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn Category;
        private DataGridViewTextBoxColumn Size;
        private DataGridViewTextBoxColumn Material;
        private DataGridViewCheckBoxColumn IsFavorite;
        private Button btnLoad1;


        public MainForm()
        {
            _dataManager = new DataManager();

            Text = Config.Instance.WindowTitle;
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            HookEvents();

            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            LoadDataFromFile();
            RefreshGrid();
        }

        private void HookEvents()
        {
            btnSearch1.Click += BtnSearch_Click;
            btnFilter1.Click += BtnFilter_Click;
            btnAdd1.Click += BtnAdd_Click;
            btnRemove1.Click += BtnRemove_Click;
            btnSave1.Click += BtnSave_Click;
            btnLoad1.Click += BtnLoad_Click;
        }

        private void LoadDataFromFile()
        {
            try
            {
                string path = Config.Instance.DataFilePath;
                _dataManager.LoadFromJson(path);
            }
            catch (DataLoadException ex)
            {
                MessageBox.Show($"Błąd ładowania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid(System.Collections.Generic.IEnumerable<Clothing>? list = null)
        {
            var toBind = list == null
                ? _dataManager.GetAllClothing().OfType<ClothingItem>().ToList()
                : list.OfType<ClothingItem>().ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = toBind;

            int count = _dataManager.GetAllClothing().Count;
            progressBar1.Maximum = WardrobeCapacity;
            progressBar1.Value = count;
            lblCapacity1.Text = $"Pojemność Garderoby: {count}/{WardrobeCapacity}";
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            try
            {
                string query = txtSearch1.Text.Trim();
                var wynik = _dataManager.SearchByName(query);
                RefreshGrid(wynik);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wyszukiwania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            try
            {
                string kategoria = ComboFilterCategory1.SelectedItem as string ?? "";
                var wynik = _dataManager.FilterByCategory(kategoria);
                RefreshGrid(wynik);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas filtrowania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_dataManager.GetAllClothing().Count >= WardrobeCapacity)
                    throw new WardrobeException($"Osiągnięto maksymalną pojemność ({WardrobeCapacity})");

                int id = 1;
                var existing = _dataManager.GetAllClothing();
                if (existing.Any())
                    id = existing.Max(c => c.Id) + 1;

                string name = txtName1.Text.Trim();
                string category = txtCategory1.SelectedItem as string ?? "";
                string color = txtColor1.Text.Trim();
                string size = txtSize1.Text.Trim();
                string material = txtMaterial1.Text.Trim();
                bool isFav = checkFavorite1.Checked;

                var item = new ClothingItem(id, name, category, color, size, material, isFav);
                _dataManager.AddClothing(item);
                RefreshGrid();

                txtName1.Clear();
                txtCategory1.SelectedIndex = -1;
                txtColor1.Clear();
                txtSize1.Clear();
                txtMaterial1.Clear();
                checkFavorite1.Checked = false;
            }
            catch (WardrobeException ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd dodawania", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            try
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Podaj Id części ubioru do wyrzucenia:", "Usuń element");
                if (!int.TryParse(input, out int id))
                    throw new WardrobeException("Nieprawidłowe Id.");

                _dataManager.RemoveClothing(id);
                RefreshGrid();
            }
            catch (WardrobeException ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd usuwania", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                string path = Config.Instance.DataFilePath;
                _dataManager.SaveToJson(path);
                MessageBox.Show("Dane zapisane pomyślnie.", "Zapis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DataLoadException ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            try
            {
                LoadDataFromFile();
                RefreshGrid();
                MessageBox.Show("Dane załadowane pomyślnie.", "Załaduj", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DataLoadException ex)
            {
                MessageBox.Show($"Błąd ładowania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn kolumna = dataGridView1.Columns[e.ColumnIndex];
            string dataProp = kolumna.DataPropertyName;

            var wszystkie = _dataManager.GetAllClothing().OfType<ClothingItem>();

            if (dataProp == nameof(ClothingItem.Name))
            {
                var sorted = wszystkie.OrderBy(item => item.Name).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.Color))
            {
                var sorted = wszystkie.OrderBy(item => item.Color).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.Category))
            {
                var sorted = wszystkie.OrderBy(item => item.Category).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.Size))
            {
                var sorted = wszystkie.OrderBy(item => item.Size).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.Material))
            {
                var sorted = wszystkie.OrderBy(item => item.Material).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.IsFavorite))
            {
                var sorted = wszystkie.OrderBy(item => item.IsFavorite).ToList();
                RefreshGrid(sorted);
            }
            else if (dataProp == nameof(ClothingItem.Id))
            {
                var sorted = wszystkie.OrderBy(item => item.Id).ToList();
                RefreshGrid(sorted);
            }
            else
            {
            }
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colColor = new DataGridViewTextBoxColumn();
            Category = new DataGridViewTextBoxColumn();
            Size = new DataGridViewTextBoxColumn();
            Material = new DataGridViewTextBoxColumn();
            IsFavorite = new DataGridViewCheckBoxColumn();
            progressBar1 = new ProgressBar();
            lblCapacity1 = new Label();
            txtName1 = new TextBox();
            txtCategory1 = new ComboBox();
            txtColor1 = new TextBox();
            txtSize1 = new TextBox();
            txtMaterial1 = new TextBox();
            checkFavorite1 = new CheckBox();
            txtSearch1 = new TextBox();
            ComboFilterCategory1 = new ComboBox();
            btnSearch1 = new Button();
            btnFilter1 = new Button();
            btnAdd1 = new Button();
            btnRemove1 = new Button();
            btnSave1 = new Button();
            btnLoad1 = new Button();
            title1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(242, 227, 196);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Id.FillWeight = 5f;
            colName.FillWeight = 23f;
            colColor.FillWeight = 17f;
            Category.FillWeight = 15f;
            Size.FillWeight = 10f;
            Material.FillWeight = 15f;
            IsFavorite.FillWeight = 10f;
            dataGridView1.BackgroundColor = Color.FromArgb(73, 26, 0);
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(210, 170, 120);
            dataGridViewCellStyle2.Font = new Font("Segoe UI Black", 9F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(20, 10, 0);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Id, colName, colColor, Category, Size, Material, IsFavorite });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Candara", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.GridColor = Color.SaddleBrown;
            dataGridView1.Location = new Point(37, 65);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(255, 248, 224);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(40, 25, 15);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(255, 248, 224);
            dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(73, 26, 0);
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridView1.Size = new Size(762, 418);
            dataGridView1.TabIndex = 0;
            // 
            // Id
            // 
            Id.DataPropertyName = "Id";
            Id.HeaderText = "ID";
            Id.Name = "Id";
            Id.ReadOnly = true;
            // 
            // colName
            // 
            colName.DataPropertyName = "Name";
            colName.HeaderText = "Nazwa";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colColor
            // 
            colColor.DataPropertyName = "Color";
            colColor.HeaderText = "Kolor";
            colColor.Name = "colColor";
            colColor.ReadOnly = true;
            // 
            // Category
            // 
            Category.DataPropertyName = "Category";
            Category.HeaderText = "Kategoria";
            Category.Name = "Category";
            Category.ReadOnly = true;
            // 
            // Size
            // 
            Size.DataPropertyName = "Size";
            Size.HeaderText = "Rozmiar";
            Size.Name = "Size";
            Size.ReadOnly = true;
            // 
            // Material
            // 
            Material.DataPropertyName = "Material";
            Material.HeaderText = "Materiał";
            Material.Name = "Material";
            Material.ReadOnly = true;
            // 
            // IsFavorite
            // 
            IsFavorite.DataPropertyName = "IsFavorite";
            IsFavorite.HeaderText = "Ulubiony";
            IsFavorite.Name = "IsFavorite";
            IsFavorite.ReadOnly = true;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.IndianRed;
            progressBar1.Location = new Point(886, 354);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(358, 47);
            progressBar1.TabIndex = 1;
            // 
            // lblCapacity1
            // 
            lblCapacity1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblCapacity1.AutoSize = true;
            lblCapacity1.BackColor = Color.White;
            lblCapacity1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            lblCapacity1.Location = new Point(957, 366);
            lblCapacity1.Name = "lblCapacity1";
            lblCapacity1.Size = new Size(82, 23);
            lblCapacity1.TabIndex = 2;
            lblCapacity1.Text = "dwadwa";
            // 
            // txtName1
            // 
            txtName1.BackColor = Color.FromArgb(35, 10, 1);
            txtName1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtName1.ForeColor = Color.FromArgb(194, 156, 118);
            txtName1.Location = new Point(44, 506);
            txtName1.Name = "txtName1";
            txtName1.PlaceholderText = "Nazwa";
            txtName1.Size = new Size(235, 30);
            txtName1.TabIndex = 3;
            // 
            // txtCategory1
            // 
            txtCategory1.BackColor = Color.FromArgb(35, 10, 1);
            txtCategory1.Cursor = Cursors.Hand;
            txtCategory1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtCategory1.ForeColor = Color.FromArgb(194, 156, 118);
            txtCategory1.FormattingEnabled = true;
            txtCategory1.Items.AddRange(new object[] { "Góra", "Dół", "Obuwie", "Akcesoria" });
            txtCategory1.Location = new Point(543, 506);
            txtCategory1.Name = "txtCategory1";
            txtCategory1.Size = new Size(256, 31);
            txtCategory1.TabIndex = 4;
            txtCategory1.Text = "  --- Wybierz kategorię ---";
            // 
            // txtColor1
            // 
            txtColor1.BackColor = Color.FromArgb(35, 10, 1);
            txtColor1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtColor1.ForeColor = Color.FromArgb(194, 156, 118);
            txtColor1.Location = new Point(300, 506);
            txtColor1.Name = "txtColor1";
            txtColor1.PlaceholderText = "Kolor";
            txtColor1.Size = new Size(226, 30);
            txtColor1.TabIndex = 5;
            // 
            // txtSize1
            // 
            txtSize1.BackColor = Color.FromArgb(35, 10, 1);
            txtSize1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtSize1.ForeColor = Color.FromArgb(194, 156, 118);
            txtSize1.Location = new Point(44, 575);
            txtSize1.Name = "txtSize1";
            txtSize1.PlaceholderText = "Rozmiar";
            txtSize1.Size = new Size(158, 30);
            txtSize1.TabIndex = 6;
            // 
            // txtMaterial1
            // 
            txtMaterial1.BackColor = Color.FromArgb(35, 10, 1);
            txtMaterial1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtMaterial1.ForeColor = Color.FromArgb(194, 156, 118);
            txtMaterial1.Location = new Point(235, 575);
            txtMaterial1.Name = "txtMaterial1";
            txtMaterial1.PlaceholderText = "Materiał";
            txtMaterial1.Size = new Size(154, 30);
            txtMaterial1.TabIndex = 7;
            // 
            // checkFavorite1
            // 
            checkFavorite1.AutoSize = true;
            checkFavorite1.BackColor = Color.FromArgb(35, 10, 1);
            checkFavorite1.BackgroundImage = Properties.Resources.button_background;
            checkFavorite1.Cursor = Cursors.Hand;
            checkFavorite1.FlatAppearance.BorderColor = Color.Lime;
            checkFavorite1.FlatAppearance.BorderSize = 3;
            checkFavorite1.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 192, 0);
            checkFavorite1.Font = new Font("Segoe UI Black", 15F, FontStyle.Bold | FontStyle.Italic);
            checkFavorite1.ForeColor = Color.FromArgb(211, 171, 125);
            checkFavorite1.Location = new Point(406, 574);
            checkFavorite1.Name = "checkFavorite1";
            checkFavorite1.Size = new Size(120, 32);
            checkFavorite1.TabIndex = 8;
            checkFavorite1.Text = "Ulubiony";
            checkFavorite1.UseVisualStyleBackColor = false;
            // 
            // txtSearch1
            // 
            txtSearch1.BackColor = Color.FromArgb(35, 10, 1);
            txtSearch1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            txtSearch1.ForeColor = Color.FromArgb(194, 156, 118);
            txtSearch1.Location = new Point(37, 18);
            txtSearch1.Name = "txtSearch1";
            txtSearch1.Size = new Size(226, 30);
            txtSearch1.TabIndex = 9;
            // 
            // ComboFilterCategory1
            // 
            ComboFilterCategory1.BackColor = Color.FromArgb(35, 10, 1);
            ComboFilterCategory1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            ComboFilterCategory1.ForeColor = Color.FromArgb(194, 156, 118);
            ComboFilterCategory1.FormattingEnabled = true;
            ComboFilterCategory1.Items.AddRange(new object[] { "Góra", "Dół", "Obuwie", "Akcesoria" });
            ComboFilterCategory1.Location = new Point(475, 17);
            ComboFilterCategory1.Name = "ComboFilterCategory1";
            ComboFilterCategory1.Size = new Size(137, 31);
            ComboFilterCategory1.TabIndex = 10;
            // 
            // btnSearch1
            // 
            btnSearch1.BackgroundImage = Properties.Resources.button_background;
            btnSearch1.Cursor = Cursors.Hand;
            btnSearch1.FlatAppearance.BorderColor = Color.FromArgb(211, 171, 125);
            btnSearch1.FlatAppearance.BorderSize = 2;
            btnSearch1.FlatStyle = FlatStyle.Flat;
            btnSearch1.Font = new Font("Segoe UI Black", 11F, FontStyle.Bold | FontStyle.Italic);
            btnSearch1.ForeColor = Color.FromArgb(211, 171, 125);
            btnSearch1.Location = new Point(269, 12);
            btnSearch1.Name = "btnSearch1";
            btnSearch1.Size = new Size(200, 41);
            btnSearch1.TabIndex = 11;
            btnSearch1.Text = "Wyszukaj po nazwie";
            btnSearch1.UseVisualStyleBackColor = true;
            // 
            // btnFilter1
            // 
            btnFilter1.BackgroundImage = Properties.Resources.button_background;
            btnFilter1.Cursor = Cursors.Hand;
            btnFilter1.FlatAppearance.BorderColor = Color.FromArgb(211, 171, 125);
            btnFilter1.FlatAppearance.BorderSize = 2;
            btnFilter1.FlatStyle = FlatStyle.Flat;
            btnFilter1.Font = new Font("Segoe UI Black", 11F, FontStyle.Bold | FontStyle.Italic);
            btnFilter1.ForeColor = Color.FromArgb(211, 171, 125);
            btnFilter1.Location = new Point(618, 12);
            btnFilter1.Name = "btnFilter1";
            btnFilter1.Size = new Size(181, 41);
            btnFilter1.TabIndex = 12;
            btnFilter1.Text = "Filtruj kategorię";
            btnFilter1.UseVisualStyleBackColor = true;
            // 
            // btnAdd1
            // 
            btnAdd1.BackColor = Color.Black;
            btnAdd1.BackgroundImage = Properties.Resources.button_background;
            btnAdd1.Cursor = Cursors.Hand;
            btnAdd1.FlatAppearance.BorderColor = Color.FromArgb(211, 171, 125);
            btnAdd1.FlatAppearance.BorderSize = 3;
            btnAdd1.FlatStyle = FlatStyle.Flat;
            btnAdd1.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold | FontStyle.Italic);
            btnAdd1.ForeColor = Color.FromArgb(211, 171, 125);
            btnAdd1.Location = new Point(543, 547);
            btnAdd1.Name = "btnAdd1";
            btnAdd1.Size = new Size(256, 60);
            btnAdd1.TabIndex = 13;
            btnAdd1.Text = "Umieść w szafie";
            btnAdd1.UseVisualStyleBackColor = false;
            // 
            // btnRemove1
            // 
            btnRemove1.BackColor = Color.FromArgb(192, 64, 0);
            btnRemove1.Cursor = Cursors.Hand;
            btnRemove1.FlatAppearance.BorderColor = Color.Black;
            btnRemove1.FlatAppearance.BorderSize = 3;
            btnRemove1.FlatStyle = FlatStyle.Flat;
            btnRemove1.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold | FontStyle.Italic);
            btnRemove1.ForeColor = Color.White;
            btnRemove1.Location = new Point(886, 181);
            btnRemove1.Name = "btnRemove1";
            btnRemove1.Size = new Size(358, 74);
            btnRemove1.TabIndex = 14;
            btnRemove1.Text = "Wyrzuć z szafy";
            btnRemove1.UseVisualStyleBackColor = false;
            // 
            // btnSave1
            // 
            btnSave1.BackgroundImage = Properties.Resources.button_background;
            btnSave1.Cursor = Cursors.Hand;
            btnSave1.FlatAppearance.BorderColor = Color.FromArgb(211, 171, 125);
            btnSave1.FlatAppearance.BorderSize = 2;
            btnSave1.FlatStyle = FlatStyle.Flat;
            btnSave1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            btnSave1.ForeColor = Color.FromArgb(211, 171, 125);
            btnSave1.Location = new Point(1074, 520);
            btnSave1.Name = "btnSave1";
            btnSave1.Size = new Size(170, 80);
            btnSave1.TabIndex = 15;
            btnSave1.Text = "Zapisz do pliku";
            btnSave1.UseVisualStyleBackColor = true;
            // 
            // btnLoad1
            // 
            btnLoad1.BackgroundImage = Properties.Resources.button_background;
            btnLoad1.Cursor = Cursors.Hand;
            btnLoad1.FlatAppearance.BorderColor = Color.FromArgb(211, 171, 125);
            btnLoad1.FlatAppearance.BorderSize = 2;
            btnLoad1.FlatStyle = FlatStyle.Flat;
            btnLoad1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            btnLoad1.ForeColor = Color.FromArgb(211, 171, 125);
            btnLoad1.Location = new Point(886, 520);
            btnLoad1.Name = "btnLoad1";
            btnLoad1.Size = new Size(170, 80);
            btnLoad1.TabIndex = 16;
            btnLoad1.Text = "Załaduj do pliku";
            btnLoad1.UseVisualStyleBackColor = true;
            // 
            // title1
            // 
            title1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            title1.AutoSize = true;
            title1.BackColor = Color.Transparent;
            title1.Font = new Font("Segoe UI Black", 38F, FontStyle.Bold | FontStyle.Italic);
            title1.ForeColor = Color.Transparent;
            title1.Location = new Point(904, 9);
            title1.Name = "title1";
            title1.Size = new Size(346, 68);
            title1.TabIndex = 17;
            title1.Text = "E-Garderoba";
            title1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            BackColor = SystemColors.ButtonFace;
            BackgroundImage = Properties.Resources.wardrobe_background;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1262, 658);
            Controls.Add(title1);
            Controls.Add(btnLoad1);
            Controls.Add(btnSave1);
            Controls.Add(btnRemove1);
            Controls.Add(btnAdd1);
            Controls.Add(btnFilter1);
            Controls.Add(btnSearch1);
            Controls.Add(ComboFilterCategory1);
            Controls.Add(txtSearch1);
            Controls.Add(checkFavorite1);
            Controls.Add(txtMaterial1);
            Controls.Add(txtSize1);
            Controls.Add(txtColor1);
            Controls.Add(txtCategory1);
            Controls.Add(txtName1);
            Controls.Add(lblCapacity1);
            Controls.Add(progressBar1);
            Controls.Add(dataGridView1);
            Name = "MainForm";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


    }
}
