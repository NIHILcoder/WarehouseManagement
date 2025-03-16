using System;
using System.Windows.Forms;
using WarehouseManagement.Forms;

namespace WarehouseManagement
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Подключаем библиотеку MaterialSkin для современного UI (требование из ТЗ)
            MaterialSkin.MaterialSkinManager materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;

            // Устанавливаем цветовую схему (Primary - основной цвет, Dark Primary - темный вариант основного, Light Primary - светлый вариант, Accent - акцентный цвет)
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.BlueGrey800,
                MaterialSkin.Primary.BlueGrey900,
                MaterialSkin.Primary.BlueGrey500,
                MaterialSkin.Accent.LightBlue200,
                MaterialSkin.TextShade.WHITE
            );

            try
            {
                // Запускаем форму входа
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                // Обработка непредвиденных исключений
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}\n\nДетали ошибки: {ex.StackTrace}",
                    "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}