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
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn Category;
        private DataGridViewTextBoxColumn Color;
        private DataGridViewTextBoxColumn Size;
        private DataGridViewTextBoxColumn Material;
        private DataGridViewCheckBoxColumn IsFavorite;
        private Label title1;
        private Button btnLoad1;

        public MainForm()
        {
            _dataManager = new DataManager();

            Text = Config.Instance.WindowTitle;
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            HookEvents();

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
            progressBar1.Value = Math.Min(count, WardrobeCapacity);
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
                string input = Microsoft.VisualBasic.Interaction.InputBox("Podaj Id elementu do usunięcia:", "Usuń element");
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

        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            Category = new DataGridViewTextBoxColumn();
            Color = new DataGridViewTextBoxColumn();
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
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Id, colName, Category, Color, Size, Material, IsFavorite });
            dataGridView1.Location = new Point(136, 90);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(919, 331);
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
            // Category
            // 
            Category.DataPropertyName = "Category";
            Category.HeaderText = "Kategoria";
            Category.Name = "Category";
            Category.ReadOnly = true;
            // 
            // Color
            // 
            Color.DataPropertyName = "Color";
            Color.HeaderText = "Kolor";
            Color.Name = "Color";
            Color.ReadOnly = true;
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
            progressBar1.Location = new Point(136, 427);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(297, 23);
            progressBar1.TabIndex = 1;
            // 
            // lblCapacity1
            // 
            lblCapacity1.AutoSize = true;
            lblCapacity1.Location = new Point(208, 430);
            lblCapacity1.Name = "lblCapacity1";
            lblCapacity1.Size = new Size(51, 15);
            lblCapacity1.TabIndex = 2;
            lblCapacity1.Text = "dwadwa";
            // 
            // txtName1
            // 
            txtName1.Location = new Point(136, 456);
            txtName1.Name = "txtName1";
            txtName1.PlaceholderText = "Nazwa";
            txtName1.Size = new Size(100, 23);
            txtName1.TabIndex = 3;
            // 
            // txtCategory1
            // 
            txtCategory1.FormattingEnabled = true;
            txtCategory1.Items.AddRange(new object[] { "Góra", "Dół", "Obuwie", "Akcesoria" });
            txtCategory1.Location = new Point(242, 456);
            txtCategory1.Name = "txtCategory1";
            txtCategory1.Size = new Size(121, 23);
            txtCategory1.TabIndex = 4;
            txtCategory1.Text = "Kategoria";
            // 
            // txtColor1
            // 
            txtColor1.Location = new Point(369, 456);
            txtColor1.Name = "txtColor1";
            txtColor1.PlaceholderText = "Kolor";
            txtColor1.Size = new Size(100, 23);
            txtColor1.TabIndex = 5;
            // 
            // txtSize1
            // 
            txtSize1.Location = new Point(475, 456);
            txtSize1.Name = "txtSize1";
            txtSize1.PlaceholderText = "Rozmiar";
            txtSize1.Size = new Size(100, 23);
            txtSize1.TabIndex = 6;
            // 
            // txtMaterial1
            // 
            txtMaterial1.Location = new Point(581, 456);
            txtMaterial1.Name = "txtMaterial1";
            txtMaterial1.PlaceholderText = "Materiał";
            txtMaterial1.Size = new Size(100, 23);
            txtMaterial1.TabIndex = 7;
            // 
            // checkFavorite1
            // 
            checkFavorite1.AutoSize = true;
            checkFavorite1.Location = new Point(687, 458);
            checkFavorite1.Name = "checkFavorite1";
            checkFavorite1.Size = new Size(74, 19);
            checkFavorite1.TabIndex = 8;
            checkFavorite1.Text = "Ulubiony";
            checkFavorite1.UseVisualStyleBackColor = true;
            // 
            // txtSearch1
            // 
            txtSearch1.Location = new Point(556, 52);
            txtSearch1.Name = "txtSearch1";
            txtSearch1.Size = new Size(100, 23);
            txtSearch1.TabIndex = 9;
            // 
            // ComboFilterCategory1
            // 
            ComboFilterCategory1.FormattingEnabled = true;
            ComboFilterCategory1.Items.AddRange(new object[] { "Góra", "Dół", "Obuwie", "Akcesoria" });
            ComboFilterCategory1.Location = new Point(809, 52);
            ComboFilterCategory1.Name = "ComboFilterCategory1";
            ComboFilterCategory1.Size = new Size(121, 23);
            ComboFilterCategory1.TabIndex = 10;
            // 
            // btnSearch1
            // 
            btnSearch1.Location = new Point(662, 52);
            btnSearch1.Name = "btnSearch1";
            btnSearch1.Size = new Size(125, 23);
            btnSearch1.TabIndex = 11;
            btnSearch1.Text = "Wyszukaj po nazwie";
            btnSearch1.UseVisualStyleBackColor = true;
            // 
            // btnFilter1
            // 
            btnFilter1.Location = new Point(936, 52);
            btnFilter1.Name = "btnFilter1";
            btnFilter1.Size = new Size(119, 23);
            btnFilter1.TabIndex = 12;
            btnFilter1.Text = "Filtruj kategorię";
            btnFilter1.UseVisualStyleBackColor = true;
            // 
            // btnAdd1
            // 
            btnAdd1.Location = new Point(767, 456);
            btnAdd1.Name = "btnAdd1";
            btnAdd1.Size = new Size(107, 23);
            btnAdd1.TabIndex = 13;
            btnAdd1.Text = "Dodaj element";
            btnAdd1.UseVisualStyleBackColor = true;
            // 
            // btnRemove1
            // 
            btnRemove1.BackColor = System.Drawing.Color.Red;
            btnRemove1.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            btnRemove1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            btnRemove1.Location = new Point(136, 485);
            btnRemove1.Name = "btnRemove1";
            btnRemove1.Size = new Size(159, 38);
            btnRemove1.TabIndex = 14;
            btnRemove1.Text = "Usuń element";
            btnRemove1.UseVisualStyleBackColor = false;
            // 
            // btnSave1
            // 
            btnSave1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            btnSave1.Location = new Point(823, 516);
            btnSave1.Name = "btnSave1";
            btnSave1.Size = new Size(172, 52);
            btnSave1.TabIndex = 15;
            btnSave1.Text = "Zapisz do pliku";
            btnSave1.UseVisualStyleBackColor = true;
            // 
            // btnLoad1
            // 
            btnLoad1.Font = new Font("Segoe UI Black", 12.75F, FontStyle.Bold | FontStyle.Italic);
            btnLoad1.Location = new Point(1001, 516);
            btnLoad1.Name = "btnLoad1";
            btnLoad1.Size = new Size(193, 52);
            btnLoad1.TabIndex = 16;
            btnLoad1.Text = "Załaduj do pliku";
            btnLoad1.UseVisualStyleBackColor = true;
            // 
            // title1
            // 
            title1.AutoSize = true;
            title1.Font = new Font("Segoe UI Black", 40F, FontStyle.Bold | FontStyle.Italic);
            title1.Location = new Point(12, 9);
            title1.Name = "title1";
            title1.Size = new Size(491, 72);
            title1.TabIndex = 17;
            title1.Text = "Twoja Garderoba";
            // 
            // MainForm
            // 
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(1206, 580);
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
