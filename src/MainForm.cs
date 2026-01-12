using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;

namespace FolderAnalyzer
{
    public class MainForm : Form
    {
        private int totalFiles = 0;
        private int totalFolders = 0;

        // Элементы управления
        private TextBox txtPath;
        private Button btnBrowse;
        private RadioButton radioWithExtension;
        private RadioButton radioWithoutExtension;
        private Button btnReport;
        private TextBox txtFileCount;
        private TextBox txtFolderCount;
        private LinkLabel linkAuthor;
        private LinkLabel linkGitHub;
        private LinkLabel linkTips;

        public MainForm()
        {
            // Устанавливаем иконку ПЕРЕД созданием формы
            SetFormIcon();

            InitializeCustomForm();
        }

        private void SetFormIcon()
        {
            try
            {
                // Способ 1: Ищем иконку рядом с EXE файлом
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exePath);

                // Попробуем разные имена файлов
                string[] possibleIconFiles =
                {
                    "FolderAnalyzer.ico",
                    "App.ico",
                    "icon.ico"
                };

                foreach (string iconFile in possibleIconFiles)
                {
                    string iconPath = Path.Combine(exeDirectory, iconFile);
                    if (File.Exists(iconPath))
                    {
                        this.Icon = new Icon(iconPath);
                        return;
                    }
                }

                // Способ 2: Извлекаем иконку из самого EXE файла
                this.Icon = Icon.ExtractAssociatedIcon(exePath);
            }
            catch
            {
                // Если не удалось загрузить иконку, оставляем стандартную
            }
        }

        private void InitializeCustomForm()
        {
            // Размер формы
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "📁 Анализатор папок";
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Padding = new Padding(20);

            // ВАЖНО: Убеждаемся, что иконка будет показана
            this.ShowIcon = true;

            // Создаем основной контейнер
            Panel mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BackColor = Color.FromArgb(37, 37, 38);
            mainContainer.BorderStyle = BorderStyle.FixedSingle;
            mainContainer.Padding = new Padding(20);

            // 1. ЗАГОЛОВОК
            Label lblTitle = new Label();
            lblTitle.Text = "📊 Анализатор структуры папок";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 122, 204);
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSize = true;
            mainContainer.Controls.Add(lblTitle);

            // 2. ПАНЕЛЬ ВЫБОРА ПАПКИ
            Panel pathPanel = new Panel();
            pathPanel.Location = new Point(10, 50);
            pathPanel.Size = new Size(520, 70);
            pathPanel.BackColor = Color.FromArgb(30, 30, 30);
            pathPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblPath = new Label();
            lblPath.Text = "📂 Выберите папку для анализа:";
            lblPath.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPath.ForeColor = Color.White;
            lblPath.Location = new Point(10, 10);
            lblPath.AutoSize = true;
            pathPanel.Controls.Add(lblPath);

            txtPath = new TextBox();
            txtPath.Location = new Point(10, 35);
            txtPath.Size = new Size(350, 25);
            txtPath.Font = new Font("Segoe UI", 9);
            txtPath.BackColor = Color.FromArgb(45, 45, 48);
            txtPath.ForeColor = Color.White;
            txtPath.BorderStyle = BorderStyle.FixedSingle;
            txtPath.ReadOnly = true;
            pathPanel.Controls.Add(txtPath);

            btnBrowse = new Button();
            btnBrowse.Text = "Обзор";
            btnBrowse.Location = new Point(370, 33);
            btnBrowse.Size = new Size(100, 28);
            btnBrowse.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBrowse.BackColor = Color.FromArgb(0, 122, 204);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Click += BtnBrowse_Click;
            pathPanel.Controls.Add(btnBrowse);

            mainContainer.Controls.Add(pathPanel);

            // 3. ПАНЕЛЬ НАСТРОЕК ОТЧЕТА с GroupBox
            GroupBox settingsGroup = new GroupBox();
            settingsGroup.Text = "📝 Формат отчета";
            settingsGroup.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            settingsGroup.ForeColor = Color.FromArgb(180, 110, 230);  // Фиолетовый цвет
            settingsGroup.Location = new Point(10, 130);
            settingsGroup.Size = new Size(520, 100);
            settingsGroup.BackColor = Color.Transparent;
            settingsGroup.FlatStyle = FlatStyle.Flat;

            // Панель внутри GroupBox для лучшего отображения
            Panel innerPanel = new Panel();
            innerPanel.Location = new Point(10, 20);
            innerPanel.Size = new Size(500, 70);
            innerPanel.BackColor = Color.FromArgb(30, 30, 30);
            innerPanel.BorderStyle = BorderStyle.FixedSingle;
            settingsGroup.Controls.Add(innerPanel);

            radioWithExtension = new RadioButton();
            radioWithExtension.Text = "С расширением файлов";
            radioWithExtension.Location = new Point(15, 15);
            radioWithExtension.Font = new Font("Segoe UI", 10);
            radioWithExtension.ForeColor = Color.White;
            radioWithExtension.BackColor = Color.Transparent;
            radioWithExtension.Checked = true;
            radioWithExtension.AutoSize = true;
            innerPanel.Controls.Add(radioWithExtension);

            radioWithoutExtension = new RadioButton();
            radioWithoutExtension.Text = "Без расширения файлов";
            radioWithoutExtension.Location = new Point(15, 40);
            radioWithoutExtension.Font = new Font("Segoe UI", 10);
            radioWithoutExtension.ForeColor = Color.White;
            radioWithoutExtension.BackColor = Color.Transparent;
            radioWithoutExtension.AutoSize = true;
            innerPanel.Controls.Add(radioWithoutExtension);

            mainContainer.Controls.Add(settingsGroup);

            // 4. КНОПКА СОЗДАНИЯ ОТЧЕТА
            btnReport = new Button();
            btnReport.Text = "📊 Создать отчет";
            btnReport.Location = new Point(180, 232);
            btnReport.Size = new Size(170, 45);
            btnReport.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnReport.BackColor = Color.FromArgb(86, 194, 86);
            btnReport.ForeColor = Color.White;
            btnReport.FlatStyle = FlatStyle.Flat;
            btnReport.FlatAppearance.BorderSize = 0;
            btnReport.Cursor = Cursors.Hand;
            btnReport.Click += BtnReport_Click;
            mainContainer.Controls.Add(btnReport);

            // 5. ПАНЕЛЬ СЧЕТЧИКОВ
            Panel countersPanel = new Panel();
            countersPanel.Location = new Point(10, 280);
            countersPanel.Size = new Size(520, 80);
            countersPanel.BackColor = Color.FromArgb(30, 30, 30);
            countersPanel.BorderStyle = BorderStyle.FixedSingle;

            // Счетчик файлов
            Label lblFiles = new Label();
            lblFiles.Text = "📄 Файлов:";
            lblFiles.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblFiles.ForeColor = Color.White;
            lblFiles.Location = new Point(30, 20);
            lblFiles.AutoSize = true;
            countersPanel.Controls.Add(lblFiles);

            txtFileCount = new TextBox();
            txtFileCount.Location = new Point(120, 18);
            txtFileCount.Size = new Size(80, 25);
            txtFileCount.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            txtFileCount.BackColor = Color.FromArgb(45, 45, 48);
            txtFileCount.ForeColor = Color.FromArgb(86, 194, 86);
            txtFileCount.Text = "0";
            txtFileCount.TextAlign = HorizontalAlignment.Center;
            txtFileCount.ReadOnly = true;
            txtFileCount.BorderStyle = BorderStyle.FixedSingle;
            countersPanel.Controls.Add(txtFileCount);

            // Счетчик папок
            Label lblFolders = new Label();
            lblFolders.Text = "📁 Папок:";
            lblFolders.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblFolders.ForeColor = Color.White;
            lblFolders.Location = new Point(240, 20);
            lblFolders.AutoSize = true;
            countersPanel.Controls.Add(lblFolders);

            txtFolderCount = new TextBox();
            txtFolderCount.Location = new Point(320, 18);
            txtFolderCount.Size = new Size(80, 25);
            txtFolderCount.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            txtFolderCount.BackColor = Color.FromArgb(45, 45, 48);
            txtFolderCount.ForeColor = Color.FromArgb(0, 122, 204);
            txtFolderCount.Text = "0";
            txtFolderCount.TextAlign = HorizontalAlignment.Center;
            txtFolderCount.ReadOnly = true;
            txtFolderCount.BorderStyle = BorderStyle.FixedSingle;
            countersPanel.Controls.Add(txtFolderCount);

            // Статус
            Label lblStatus = new Label();
            lblStatus.Text = "Статус: Готов к работе";
            lblStatus.Font = new Font("Segoe UI", 9);
            lblStatus.ForeColor = Color.LightGray;
            lblStatus.Location = new Point(30, 50);
            lblStatus.AutoSize = true;
            countersPanel.Controls.Add(lblStatus);

            mainContainer.Controls.Add(countersPanel);

            // 6. ССЫЛКИ - В САМОМ НИЗУ ФОРМЫ
            Panel linksPanel = new Panel();
            linksPanel.Location = new Point(10, 380);
            linksPanel.Size = new Size(520, 50);
            linksPanel.BackColor = Color.Transparent;

            // Ссылка на автора
            linkAuthor = new LinkLabel();
            linkAuthor.Text = "👤 Автор";
            linkAuthor.Location = new Point(30, 10);
            linkAuthor.AutoSize = true;
            linkAuthor.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            linkAuthor.LinkColor = Color.FromArgb(0, 122, 204);
            linkAuthor.VisitedLinkColor = Color.FromArgb(0, 122, 204);
            linkAuthor.ActiveLinkColor = Color.FromArgb(0, 150, 255);
            linkAuthor.Cursor = Cursors.Hand;
            linkAuthor.Click += (s, e) => OpenUrl("https://my.ws-soft.ru");
            linksPanel.Controls.Add(linkAuthor);

            // Разделитель
            Label separator1 = new Label();
            separator1.Text = "|";
            separator1.Location = new Point(100, 10);
            separator1.AutoSize = true;
            separator1.Font = new Font("Segoe UI", 9);
            separator1.ForeColor = Color.Gray;
            linksPanel.Controls.Add(separator1);

            // Ссылка на GitHub
            linkGitHub = new LinkLabel();
            linkGitHub.Text = "🐙 GitHub";
            linkGitHub.Location = new Point(120, 10);
            linkGitHub.AutoSize = true;
            linkGitHub.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            linkGitHub.LinkColor = Color.FromArgb(180, 110, 230);
            linkGitHub.VisitedLinkColor = Color.FromArgb(180, 110, 230);
            linkGitHub.ActiveLinkColor = Color.FromArgb(200, 130, 255);
            linkGitHub.Cursor = Cursors.Hand;
            linkGitHub.Click += (s, e) => OpenUrl("https://github.com/wulkan-Git");
            linksPanel.Controls.Add(linkGitHub);

            // Разделитель
            Label separator2 = new Label();
            separator2.Text = "|";
            separator2.Location = new Point(210, 10);
            separator2.AutoSize = true;
            separator2.Font = new Font("Segoe UI", 9);
            separator2.ForeColor = Color.Gray;
            linksPanel.Controls.Add(separator2);

            // Ссылка на поддержку
            linkTips = new LinkLabel();
            linkTips.Text = "💝 Поддержать проект";
            linkTips.Location = new Point(230, 10);
            linkTips.AutoSize = true;
            linkTips.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            linkTips.LinkColor = Color.FromArgb(255, 90, 90);
            linkTips.VisitedLinkColor = Color.FromArgb(255, 90, 90);
            linkTips.ActiveLinkColor = Color.FromArgb(255, 120, 120);
            linkTips.Cursor = Cursors.Hand;
            linkTips.Click += (s, e) => ShowTipForm();
            linksPanel.Controls.Add(linkTips);

            mainContainer.Controls.Add(linksPanel);

            // Добавляем основной контейнер на форму
            this.Controls.Add(mainContainer);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для анализа";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = folderDialog.SelectedPath;
                    totalFiles = 0;
                    totalFolders = 0;
                    txtFileCount.Text = "0";
                    txtFolderCount.Text = "0";
                }
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text) || !Directory.Exists(txtPath.Text))
            {
                MessageBox.Show("Выберите существующую папку!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                totalFiles = 0;
                totalFolders = 0;

                // Создание отчета
                string reportPath = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.Desktop), $"Отчет_папки_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (var writer = new StreamWriter(reportPath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine($"Отчет по структуре папки: {txtPath.Text}");
                    writer.WriteLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    writer.WriteLine(new string('-', 50));

                    ScanDirectory(txtPath.Text, "", writer);
                }

                // Обновление счетчиков
                txtFileCount.Text = totalFiles.ToString();
                txtFolderCount.Text = totalFolders.ToString();

                MessageBox.Show($"✅ Отчет успешно создан!\n\n📄 Файлов: {totalFiles}\n📁 Папок: {totalFolders}\n\n📋 Отчет сохранен на рабочем столе.",
                    "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка при создании отчета:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScanDirectory(string path, string indent, StreamWriter writer)
        {
            try
            {
                // Подсчет и запись файлов в текущей папке
                var files = Directory.GetFiles(path);
                totalFiles += files.Length;

                foreach (var file in files)
                {
                    string fileName = radioWithExtension.Checked ?
                        Path.GetFileName(file) :
                        Path.GetFileNameWithoutExtension(file);
                    writer.WriteLine($"{indent}📄 {fileName}");
                }

                // Рекурсивный обход подпапок
                var directories = Directory.GetDirectories(path);
                totalFolders += directories.Length;

                foreach (var dir in directories)
                {
                    string dirName = Path.GetFileName(dir);
                    writer.WriteLine($"{indent}📁 {dirName}");
                    ScanDirectory(dir, indent + "  ", writer);
                }
            }
            catch (UnauthorizedAccessException)
            {
                writer.WriteLine($"{indent}⛔ [Доступ запрещен]");
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Не удалось открыть ссылку:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTipForm()
        {
            var tipForm = new TipForm();
            tipForm.ShowDialog();
        }
    }
}