using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;

namespace FolderAnalyzer
{
    public class TipForm : Form
    {
        private PictureBox qrPictureBox;

        public TipForm()
        {
            // Устанавливаем иконку ПЕРЕД созданием формы
            SetFormIcon();

            InitializeTipForm();
        }

        private void SetFormIcon()
        {
            try
            {
                // Используем ту же иконку, что и в MainForm
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

                // Если не нашли файл, извлекаем иконку из EXE
                this.Icon = Icon.ExtractAssociatedIcon(exePath);
            }
            catch
            {
                // Оставляем стандартную иконку, если не удалось загрузить
            }
        }

        private void InitializeTipForm()
        {
            // Настройки формы
            this.Text = "💝 Поддержать автора";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Padding = new Padding(20);

            // ВАЖНО: Убеждаемся, что иконка будет показана
            this.ShowIcon = true;

            // Основной контейнер
            Panel mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BackColor = Color.FromArgb(37, 37, 38);
            mainContainer.BorderStyle = BorderStyle.FixedSingle;
            mainContainer.Padding = new Padding(20);

            // Заголовок
            Label lblTitle = new Label();
            lblTitle.Text = "💝 Поддержать автора";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(255, 90, 90);
            lblTitle.Location = new Point(80, 20);
            lblTitle.AutoSize = true;
            mainContainer.Controls.Add(lblTitle);

            // Текст благодарности
            Label lblMessage = new Label();
            lblMessage.Text = "Если этот проект был полезен для вас, и вы хотите поддержать дальнейшую разработку, \n" +
                             "вы можете сделать это через представленные ниже способы.\n\n" +
                             "Ваша поддержка очень важна \n и мотивирует на создание новых полезных инструментов!";
            lblMessage.Font = new Font("Segoe UI", 10);
            lblMessage.ForeColor = Color.White;
            lblMessage.Location = new Point(20, 70);
            lblMessage.Size = new Size(550, 80);
            lblMessage.TextAlign = ContentAlignment.MiddleLeft;
            mainContainer.Controls.Add(lblMessage);

            // Ссылка
            LinkLabel linkLabel = new LinkLabel();
            linkLabel.Text = "🔗 Поддержать";
            linkLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            linkLabel.LinkColor = Color.FromArgb(0, 122, 204);
            linkLabel.VisitedLinkColor = Color.FromArgb(0, 122, 204);
            linkLabel.ActiveLinkColor = Color.FromArgb(0, 150, 255);
            linkLabel.Location = new Point(60, 160);
            linkLabel.AutoSize = true;
            linkLabel.Cursor = Cursors.Hand;
            linkLabel.Click += (s, e) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://dalink.to/w_u_l_k_a_n",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            mainContainer.Controls.Add(linkLabel);

            // Создаем PictureBox для QR-кода
            qrPictureBox = new PictureBox();
            qrPictureBox.Location = new Point(130, 200);
            qrPictureBox.Size = new Size(150, 150);
            qrPictureBox.BackColor = Color.White;
            qrPictureBox.BorderStyle = BorderStyle.FixedSingle;
            qrPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            // Загружаем QR-код из ресурсов
            LoadQrCode();

            mainContainer.Controls.Add(qrPictureBox);

            // Кнопка закрытия
            Button btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.BackColor = Color.FromArgb(0, 122, 204);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Size = new Size(120, 35);
            btnClose.Location = new Point(155, 360);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            mainContainer.Controls.Add(btnClose);

            this.Controls.Add(mainContainer);
        }

        private void LoadQrCode()
        {
            try
            {
                // Способ 1: Через Resources.resx
                if (Properties.Resources.qr_code != null)
                {
                    qrPictureBox.Image = Properties.Resources.qr_code;
                    return;
                }
            }
            catch
            {
                // Продолжаем попытки
            }

            try
            {
                // Способ 2: Из Embedded Resources
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Попробуем найти ресурс с именем содержащим "qr_code"
                string[] resourceNames = assembly.GetManifestResourceNames();
                string qrResourceName = null;

                foreach (string name in resourceNames)
                {
                    if (name.ToLower().Contains("qr_code") && name.ToLower().EndsWith(".png"))
                    {
                        qrResourceName = name;
                        break;
                    }
                }

                if (qrResourceName != null)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(qrResourceName))
                    {
                        if (stream != null)
                        {
                            qrPictureBox.Image = Image.FromStream(stream);
                            return;
                        }
                    }
                }
            }
            catch
            {
                // Продолжаем попытки
            }

            try
            {
                // Способ 3: Ищем файл рядом с EXE
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exePath);
                string qrFilePath = Path.Combine(exeDirectory, "qr_code.png");

                if (File.Exists(qrFilePath))
                {
                    qrPictureBox.Image = Image.FromFile(qrFilePath);
                    return;
                }
            }
            catch
            {
                // Продолжаем попытки
            }

            // Если все способы не сработали, показываем заглушку
            ShowNoQrCode();
        }

        private void ShowNoQrCode()
        {
            // Создаем изображение с текстом
            Bitmap placeholder = new Bitmap(150, 150);
            using (Graphics g = Graphics.FromImage(placeholder))
            {
                g.Clear(Color.White);

                // Рисуем рамку
                using (Pen pen = new Pen(Color.LightGray, 2))
                {
                    g.DrawRectangle(pen, 1, 1, 147, 147);
                }

                // Рисуем крестик
                using (Pen pen = new Pen(Color.DarkGray, 3))
                {
                    g.DrawLine(pen, 30, 30, 120, 120);
                    g.DrawLine(pen, 120, 30, 30, 120);
                }

                // Текст
                using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.DarkGray))
                {
                    string text = "QR-код\nне найден";
                    SizeF textSize = g.MeasureString(text, font);
                    float x = (150 - textSize.Width) / 2;
                    float y = (150 - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }

            qrPictureBox.Image = placeholder;
        }
    }
}