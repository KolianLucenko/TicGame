using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using CoolGameLibrary;
using System.Reflection;

namespace MyCoolGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Двумерный массив с путями к картинкам
        string[,] UserImage;
        // Двумерный массив карты игры
        int[,] GameMap;
        // Индекстатор для проверок
        int[] InArr;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                PrepareNewGame();
                CanvasGame.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        // Нажатие на полотно
        private async void Click(object sender, MouseButtonEventArgs e)
        {
            if (CanvasGame.IsEnabled)
            {
                CanvasGame.IsEnabled = false;
                Point pt = e.GetPosition(this.CanvasGame);
                // Получить правильные координаты и индексатор для проверки
                Point newP = Game.GetPoint(pt, out InArr);
                // Если ячека пустая
                if (GameMap[InArr[0], InArr[1]] == 0)
                {

                    // Отметить ячейку
                    GameMap[InArr[0], InArr[1]] = 1;

                    // Загрузить картинку
                    Game.AddImage(newP, UserImage, PlayerType.User, CanvasGame);
                    WinType wt = Game.CheckWin(GameMap, 1);
                    if (wt != WinType.NULL)
                    {
                        Game.GetImage(CanvasGame, wt);
                        await GameResult(1);
                        return;
                    }
                    // Проверка на ничь
                    if (Game.ChekingDeadHeat(GameMap))
                    {
                        await GameResult(3);
                    }

                    // ---------------- ХОД КОМПЬЮТЕРА -----------------------//
                    await EaseComputer();
                    //Получить координаты
                    // Загрузить картинку
                    Game.AddImage(Game.ComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);
                    wt = Game.CheckWin(GameMap, -1);
                    if (wt != WinType.NULL)
                    {
                        Game.GetImage(CanvasGame, wt);
                        await GameResult(2);
                        return;
                    }
                    // Проверка на ничь
                    if (Game.ChekingDeadHeat(GameMap))
                    {
                        await GameResult(3);
                        return;
                    }
                    CanvasGame.IsEnabled = true;
                }
            }
        }

        // Имитация принятия решения
        public async Task EaseComputer()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1200);
            });
        }

        /// <summary>
        /// Вывод результата игры на экран и подготовка к новой игре
        /// </summary>
        /// <returns></returns>
        public async Task GameResult(int i)
        {
            await EaseComputer();

            await Task.Run(() =>
            {
                if (i == 1)
                    MessageBox.Show("Вы выиграли");
                else if(i == 2)
                    MessageBox.Show("Вы проиграли");
                else if(i == 3)
                    MessageBox.Show("Ничья");
            });
            ClearVariable();
            PrepareNewGame();
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Начать новую игру?", "Yes/No", MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                ClearVariable();
                PrepareNewGame();
            }

        }        

        /// <summary>
        /// Очистить все переменные
        /// </summary>
        private void ClearVariable()
        {
            string[,] UserImage = null;
            int[,] GameMap = null;
            int[] InArr = null;
        }

        /// <summary>
        /// Подготовка приложения к  новой игре
        /// </summary>
        private void PrepareNewGame()
        {
            Game.FillArray(@"Resource", out UserImage);
            Game.FillMap(out GameMap);
            // Очистить полотно
            CanvasGame.Children.Clear();
            CanvasGame.IsEnabled = false;

            
        }

        /// <summary>
        /// Кнопка старта игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            CanvasGame.IsEnabled = true;
            Game.AddImage(Game.ComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);
        }
  

    }
}
