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
using System.Configuration;

namespace MyCoolGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Двумерный массив с путями к картинкам
        /// </summary>
        string[,] UserImage;
        /// <summary>
        /// Двумерный массив карты игры
        /// </summary>
        int[,] GameMap;
        /// <summary>
        /// Индекстатор для проверок
        /// </summary>
        int[] InArr;
        /// <summary>
        /// уровень противника
        /// </summary>
        string level = "Ease";
        /// <summary>
        /// Статистика игрока
        /// </summary>
        public Statistic stat;


        /// <summary>
        /// Выполнение при запуске программы
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                PrepareNewGame();
                CanvasGame.IsEnabled = false;
                stat = Game.LoadStatistic();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        /// <summary>
        /// Нажатие на полотно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Click(object sender, MouseButtonEventArgs e)
        {
            if (CanvasGame.IsEnabled)
            {
                
                Point pt = e.GetPosition(this.CanvasGame);
                // Получить правильные координаты и индексатор для проверки
                Point newP = Game.GetPoint(pt, out InArr);
                // Если ячека пустая
                if (GameMap[InArr[0], InArr[1]] == 0)
                {
                    CanvasGame.IsEnabled = false;

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
                        return;
                    }

                    // ---------------- ХОД КОМПЬЮТЕРА -----------------------//
                    await ImitationDecision();
                    //Получить координаты
                    // Загрузить картинку
                    if(level == "Ease")
                        Game.AddImage(Game.EaseComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);
                    if(level == "Normal")
                        Game.AddImage(Game.NormalComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);
                    if (level == "Hard")
                        Game.AddImage(Game.HardComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);
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

        /// <summary>
        /// Имитация принятия решения (задержка на заданное время)
        /// </summary>
        /// <returns>Ничего не возвращает</returns>
        public async Task ImitationDecision(int Time = 1200)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(Time);
            });
        }

        /// <summary>
        /// Вывод результата игры на экран и подготовка к новой игре
        /// </summary>
        /// <returns></returns>
        public async Task GameResult(int i)
        {
            await ImitationDecision();

            await Task.Run(() =>
            {
                if (i == 1)
                {
                    MessageBox.Show("Вы выиграли");
                    switch(level){
                        case "Ease":
                            stat.WinEase += 1;break;
                        case "Normal":
                            stat.WinNormal += 1;break;
                        case "Hard":
                            stat.WinHard += 1;break;
                    }
                }
                else if(i == 2)
                {
                    MessageBox.Show("Вы проиграли");
                    switch (level)
                    {
                        case "Ease":
                            stat.OverEase += 1; break;
                        case "Normal":
                            stat.OverNormal += 1; break;
                        case "Hard":
                            stat.OverHard += 1; break;
                    }
                }
                else if (i == 3)
                {
                    MessageBox.Show("Ничья");
                    switch (level)
                    {
                        case "Ease":
                            stat.DrawEase += 1; break;
                        case "Normal":
                            stat.DrawNormal += 1; break;
                        case "Hard":
                            stat.DrawHard += 1; break;
                    }
                }
            });
            try
            {
                ClearVariable();
                PrepareNewGame();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        /// <summary>
        /// Начать новую игру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            Game.SaveStatistic(stat);
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

            Ease.IsEnabled = true;
            Normal.IsEnabled = true;
            Hard.IsEnabled = false;

            StartGameButton.IsEnabled = true;
        }

        /// <summary>
        /// Кнопка старта игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            AskLevel Ask = new AskLevel();
            Ask.ShowDialog();
            if (Ask.DialogResult == true)
            {
                level = Ask.GetLevel();
            }
            else
            {
                MessageBox.Show("Вы не выбрали сложность игры.\nПовторите ещё раз.", "Ошибка");
                return;
            }

            CanvasGame.IsEnabled = true;
            Game.AddImage(Game.EaseComputerRun(ref GameMap), UserImage, PlayerType.Computer, CanvasGame);

            StartGameButton.IsEnabled = false;
        }


        /// <summary>
        /// Установить сложность игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetComplexity(object sender, RoutedEventArgs e)
        {
            Ease.IsChecked = false;
            Normal.IsChecked = false;
            Hard.IsChecked = false;

            MenuItem m = sender as MenuItem;
            m.IsChecked = true;
            level = m.Tag.ToString();
        }

        /// <summary>
        /// Проверка - началась ли игра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            for(int i=0;i<GameMap.GetLength(0);i++)
                for(int c=0;c<GameMap.GetLength(1);c++)
                    if(GameMap[i,c]!=0)
                    {
                        Ease.IsEnabled = false;
                        Normal.IsEnabled = false;
                        Hard.IsEnabled = false;
                    }
        }

        /// <summary>
        /// Вывести статистику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Statistics(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(stat.ToString(),"Статистика");
        }
    }
}
