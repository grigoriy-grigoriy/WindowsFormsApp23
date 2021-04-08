using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp23

{//------------ Эта простая (вымышленная) модель программы здесь немного логики чтобы показать работу программы и взаимодействия между компонентами-----------------------

    public partial class Form1 : Form
    {
        //Курсы валют:
        double EUR = 32.0;
        double USD = 28.0;

       
        public Form1()
        {
            InitializeComponent();
            
        }
       //работа с плитками на форме
           private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
           {
           
           if (e.TabPageIndex == (tabControl1.TabPages.Count - 1))
               {
                   tabControl1.TabPages.Insert(tabControl1.TabPages.Count - 1, "Легковий");
               
               
                e.Cancel = true;
             
                }  
       
           }
        //-----------------расчет плитки 1 (легковая)----------------
        private void buttonCalulate_Click(object sender, EventArgs e)
        {
          
            //0. Если пользователь забыл ввести данные:
            if (this.textBox3.Text == ""  || textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox12.Text == "")
            {
                //Выводим ошибку
                MessageBox.Show("Данные не могут быть считаны. \n\nЗаполните пожалуйста все поля.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //Останавливаем работу функции
            }
          
            int age = 0;    // Возраст автомобиля
           
                  int Box3 = 0; // Объём двигателя
               bool isDiesel = false;  // Дизельный двигатель?
            int priceUser = 0;      // Стоимость автомобиля в валюте пользователя
          int priceGRN = 0;       // Стоимость автомобиля в GRN
       string currency = "Грн";// Выбранная пользователем валюта

            //_____1. Считывание данных с формы-плитки № 1:

            // Задаём возраст автомобиля (age):
            if (comboBox3.Text == "До 3 лет")
                age = 0;
            else if (comboBox3.Text == "От 3 до 5 лет")
                age = 3;
            else if (comboBox3.Text == "От 5 до 7 лет")
                age = 6;
            else if (comboBox3.Text == "Более 15 лет")
                age = 10;

            // Задаём выбранную пользователем валюту (currency):
            if (comboBox12.Text == "Грн.")
                currency = "Грн";
            else if (comboBox12.Text == "Евро")
            {
                currency = "EUR";
                EUR = Convert.ToDouble(numericUpDown1.Value);
            }
            else if (comboBox12.Text == "Долл.")
            {
                currency = "USD";
                USD = Convert.ToDouble(numericUpDown1.Value);
            }

            // Устанавливаем остальные параметры:
          
            Box3 = Convert.ToInt32(this.textBox3.Text);

            if (comboBox2.Text == "Дизельный")
                isDiesel = true;

            // Считываем цену в валюте пользователя:
          priceUser = Convert.ToInt32(textBox2.Text);
            // Переводим валюту пользователя в Грн:
            if (currency == "Грн")
                priceGRN = priceUser;
            else if (currency == "EUR")
                priceGRN = Convert.ToInt32(priceUser * EUR);
            else if (currency == "USD")
                priceGRN = Convert.ToInt32(priceUser * USD);

            // (все принимают на вход сумму в гривнях)

            //_                Расчёт данных:
           
            int mainTax = Calc_MainTax( age, Box3, isDiesel, priceGRN); // Таможенная пошлина
          
            
            int excise = 0;
            int NDS = 0;
          
                //Рассчитываем акциз и НДС:
                excise = Calc_Excise(Box3);
                NDS = Calc_NDS(priceGRN, mainTax, excise);
          
           
            //______3. Вартість авто із розмитненням:
                 labelTaxResult.Text = String.Format("{0:N0}", mainTax) + " Грн.";
           
            labelExciseResult.Text = String.Format("{0:N0}", excise) + " Грн.";
            
            labelNDSResult.Text = String.Format("{0:N0}", NDS) + " Грн.";

            //Расчёт итоговой стоимости:
            int finalresult = mainTax + excise + NDS;
            labelFinalResult.Text = String.Format("{0:N0}", finalresult) + " Грн.";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // легковые
            comboBox3.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox12.SelectedIndex = 0;
                button1.Text = "\u2699";
           // автобусы
            comboBox7.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            button2.Text = "\u2699";
           // грузовые
            comboBox11.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox13.SelectedIndex = 0;
            button3.Text = "\u2699";
            // мото
           
            comboBox15.SelectedIndex = 0;
            comboBox16.SelectedIndex = 0;
            button6.Text = "\u2699";
        }

        //Основная пошлина:
        private int Calc_MainTax( int age, int engine, bool isDiesel, int priceGRN)
        {
            double fee = 0; //Пошлина
            double minimumFee; //Минимальный размер пошлины для рассматриваемого авто

            int priceEUR = priceGRN / 32; // Цена в евро

            double percent = 0; //Процент от стоимости авто
            double minCoef = 0; //Минимальный коэффициент евро/см (относительно объёма двигателя)

           
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (priceEUR <= 5500)
                    {
                        percent = 0.54;
                        minCoef = 2.5;
                    }
                    else if (priceEUR <= 10000)
                    {
                        percent = 0.48;
                        minCoef = 3.5;
                    }
                    else if (priceEUR <= 32000)
                    {
                        percent = 0.48;
                        minCoef = 5.5;
                    }
                    else if (priceEUR <= 84500)
                    {
                        percent = 0.48;
                        minCoef = 7.5;
                    }
                    else if (priceEUR <= 169000)
                    {
                        percent = 0.48;
                        minCoef = 15;
                    }
                    else if (priceEUR > 169000)
                    {
                        percent = 0.48;
                        minCoef = 20;
                    }
                }
                //От 3 до 5 лет:
                else if (age > 3 && age <= 5)
                {
                    if (engine <= 1000)
                        minCoef = 1.5;
                    else if (engine <= 1500)
                        minCoef = 1.7;
                    else if (engine <= 1800)
                        minCoef = 2.5;
                    else if (engine <= 2300)
                        minCoef = 2.7;
                    else if (engine <= 3000)
                        minCoef = 3;
                    else if (engine > 3000)
                        minCoef = 3.6;
                }
                //Больше 5 лет:
                else if (age > 5)
                {
                    if (engine <= 1000)
                        minCoef = 3;
                    else if (engine <= 1500)
                        minCoef = 3.2;
                    else if (engine <= 1800)
                        minCoef = 3.5;
                    else if (engine <= 2300)
                        minCoef = 4.8;
                    else if (engine <= 3000)
                        minCoef = 5;
                    else if (engine > 3000)
                        minCoef = 5.7;
                }
           
                if (!isDiesel)
                {
                    //Меньше 3 лет:
                    if (age <= 3)
                    {
                        if (engine <= 1000)
                        {
                            percent = 0.23;
                            minCoef = 0.67;
                        }
                        else if (engine <= 1500)
                        {
                            percent = 0.23;
                            minCoef = 0.73;
                        }
                        else if (engine <= 1800)
                        {
                            percent = 0.23;
                            minCoef = 0.83;
                        }
                        else if (engine <= 2300)
                        {
                            percent = 0.23;
                            minCoef = 1.2;
                        }
                        else if (engine <= 3000)
                        {
                            percent = 0.23;
                            minCoef = 1.2;
                        }
                        else if (engine > 3000)
                        {
                            percent = 0.23;
                            minCoef = 1.57;
                        }
                    }
                    //От 3 до 7 лет:
                    else if (age > 3 && age <= 7)
                    {
                        if (engine <= 1000)
                        {
                            percent = 0.25;
                            minCoef = 0.45;
                        }
                        else if (engine <= 1500)
                        {
                            percent = 0.25;
                            minCoef = 0.5;
                        }
                        else if (engine <= 1800)
                        {
                            percent = 0.25;
                            minCoef = 0.45;
                        }
                        else if (engine <= 2300)
                        {
                            percent = 0.25;
                            minCoef = 0.55;
                        }
                        else if (engine <= 3000)
                        {
                            percent = 0.25;
                            minCoef = 0.55;
                        }
                        else if (engine > 3000)
                        {
                            percent = 0.25;
                            minCoef = 1;
                        }
                    }
                    //Старше 7 лет:
                    else if (age > 7)
                    {
                        if (engine <= 1000)
                            minCoef = 1.4;
                        else if (engine <= 1500)
                            minCoef = 1.5;
                        else if (engine <= 1800)
                            minCoef = 1.6;
                        else if (engine <= 2300)
                            minCoef = 2.2;
                        else if (engine <= 3000)
                            minCoef = 3.2;
                        else if (engine > 3000)
                            minCoef = 3.2;
                    }
                }    
        
                else
                if (isDiesel)
                {
                    //Меньше 3 лет:
                    if (age <= 3)
                    {
                        if (engine <= 1500)
                        {
                            percent = 0.23;
                            minCoef = 0.8;
                        }
                        else if (engine <= 2500)
                        {
                            percent = 0.23;
                            minCoef = 1.2;
                        }
                        else if (engine > 2500)
                        {
                            percent = 0.23;
                            minCoef = 1.57;
                        }
                    }
                    //От 3 до 7 лет:
                    else if (age > 3 && age <= 7)
                    {
                        if (engine <= 1500)
                        {
                            percent = 0.25;
                            minCoef = 0.4;
                        }
                        else if (engine <= 2500)
                        {
                            percent = 0.25;
                            minCoef = 0.5;
                        }
                        else if (engine > 2500)
                        {
                            percent = 0.25;
                            minCoef = 1;
                        }
                    }
                    //Старше 7 лет:
                    else if (age > 7)
                    {
                        if (engine <= 1500)
                            minCoef = 1.5;
                        else if (engine <= 2500)
                            minCoef = 2.2;
                        else if (engine > 2500)
                            minCoef = 3.2;
                    }
        
                }
          

            //Вычисляем таможенную ставку (в евро):
            fee = priceEUR * percent;
            minimumFee = engine * minCoef;

            //Если ставка ниже минимально разрешённого порога, принимаем порог:
            if (fee < minimumFee)
                fee = minimumFee;

            //Переводим Грн в евро:
            fee = fee * 32;

            return Convert.ToInt32(fee);
        }
   
        //Акциз:
        private int Calc_Excise(int engineCapacity )
        {
            int excise = 0;    //Акциз
            int priceCoef = 0; //Коэффициент Грн\объем двигателя

            //Задаём коэффициент на основании объема двигателя:
            if (engineCapacity  <= 900)
                priceCoef = 0;
            else if (engineCapacity  <= 1500)
                priceCoef = 3;
            else if (engineCapacity  <= 2000)
                priceCoef = 7;
            else if (engineCapacity  <= 3000)
                priceCoef = 9;
            else if (engineCapacity  <= 4000)
                priceCoef = 12;
            else if (engineCapacity  <= 5000)
                priceCoef = 16;
            else if (engineCapacity  > 5000)
                priceCoef = 25;

            //Рассчитываем акциз:
            excise = engineCapacity  * priceCoef;

            return excise;
        }

        //НДС:
        private int Calc_NDS(int price, int mainTax, int excise)
        {
            // НДС (20%) расчитывается от суммы: стоимость авто + таможенная пошлина + акциз.
            double NDS;

            NDS = (price + mainTax + excise) * 0.20;

            return Convert.ToInt32(NDS);
        }

        #region Проверка_вводимых_символов

        //Функция, проверяющая напечатанный символ:
        private void HandleKey_Number(KeyPressEventArgs e)
        {
            //Если клавиша - это цифра или backspace:
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
#pragma warning disable CS0642 // Возможно, ошибочный пустой оператор
                ;
#pragma warning restore CS0642 // Возможно, ошибочный пустой оператор
                              // Разрешаем напечатать символ

                //Если мы нажали не цифру, то запрещаем печатать символ:
            else e.Handled = true;
        }

        //Функции, ловящие событие "печать символа" (KeyPress):
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);

        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);
        }
        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);
        }

        #endregion Проверка_вводимых_символов


        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox12.Text == "Грн.")
                numericUpDown1.Text = "1";
            else if (comboBox12.Text == "Евро")
                numericUpDown1.Text = EUR.ToString();
            else if (comboBox12.Text == "Долл.")
                numericUpDown1.Text = USD.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            button1.Visible = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 32)
                numericUpDown1.Value = 32;

            if (comboBox12.Text == "Евро")
                EUR = (double)numericUpDown1.Value;
            else if (comboBox12.Text == "Долл.")
                USD = (double)numericUpDown1.Value;
        }
        //--------------------------------------------------------------------------------------------------------------------------  
       
        //-----------------расчет плитки 3 (грузовая)----------------
        private void button4_Click(object sender, EventArgs e)
        {

            //0. Если пользователь забыл ввести данные:
            if (this.textBox1.Text == "" || textBox6.Text == "" || comboBox4.Text == "" || comboBox10.Text == "" || comboBox11.Text == "" || comboBox9.Text == ""|| comboBox13.Text == "" || comboBox14.Text == "")
            {
                //Выводим ошибку
                MessageBox.Show("Данные не могут быть считаны. \n\nЗаполните пожалуйста все поля.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //Останавливаем работу функции
            }

            int age = 0;    // Возраст автомобиля

            int Box3 = 0; // Объём двигателя
            bool isDiesel = false;  // Дизельный двигатель?
            int priceUser = 0;      // Стоимость автомобиля в валюте пользователя
            int priceGRN = 0;       // Стоимость автомобиля в GRN
            string currency = "Грн";// Выбранная пользователем валюта

            //_____1. Считывание данных с формы-плитки № 1:

            // Задаём возраст автомобиля (age):
            if (comboBox11.Text == "До 3 лет")
                age = 0;
            else if (comboBox11.Text == "От 3 до 5 лет")
                age = 3;
            else if (comboBox11.Text == "От 5 до 7 лет")
                age = 6;
            else if (comboBox11.Text == "Более 15 лет")
                age = 10;

            // Задаём выбранную пользователем валюту (currency):
            if (comboBox13.Text == "Грн.")
                currency = "Грн";
            else if (comboBox13.Text == "Евро")
            {
                currency = "EUR";
                EUR = Convert.ToDouble(numericUpDown3.Value);
            }
            else if (comboBox13.Text == "Долл.")
            {
                currency = "USD";
                USD = Convert.ToDouble(numericUpDown3.Value);
            }

            // Устанавливаем остальные параметры:

            Box3 = Convert.ToInt32(this.textBox6.Text);

            if (comboBox10.Text == "Дизельный")
                isDiesel = true;

            // Считываем цену в валюте пользователя:
            priceUser = Convert.ToInt32(textBox1.Text);
            // Переводим валюту пользователя в Грн:
            if (currency == "Грн")
                priceGRN = priceUser;
            else if (currency == "EUR")
                priceGRN = Convert.ToInt32(priceUser * EUR);
            else if (currency == "USD")
                priceGRN = Convert.ToInt32(priceUser * USD);

            // (все принимают на вход сумму в гривнях)

            //_                Расчёт данных:

            int mainTax = Calc2_MainTax(age, Box3, isDiesel, priceGRN); // Таможенная пошлина

            
            int excise = 0;
            int NDS = 0;
            
            //Рассчитываем акциз и НДС:
            excise = Calc2_Excise(Box3);
            NDS = Calc2_NDS(priceGRN, mainTax, excise);
            //  }

            //______3. Вартість авто із розмитненням:
            labelTaxResult.Text = String.Format("{0:N0}", mainTax) + " Грн.";

            labelExciseResult.Text = String.Format("{0:N0}", excise) + " Грн.";

            labelNDSResult.Text = String.Format("{0:N0}", NDS) + " Грн.";

            //Расчёт итоговой стоимости:
            int finalresult = mainTax + excise + NDS;
            labelFinalResult.Text = String.Format("{0:N0}", finalresult) + " Грн.";
        }

        

        //Основная пошлина:
        private int Calc2_MainTax(int age, int engine, bool isDiesel, int priceGRN)
        {
            double fee = 0; //Пошлина
            double minimumFee; //Минимальный размер пошлины для рассматриваемого авто

            int priceEUR = priceGRN / 32; // Цена в евро

            double percent = 0; //Процент от стоимости авто
            double minCoef = 0; //Минимальный коэффициент евро/см (относительно объёма двигателя)


            //Меньше 3 лет:
            if (age <= 3)
            {
                if (priceEUR <= 5500)
                {
                    percent = 0.54;
                    minCoef = 2.5;
                }
                else if (priceEUR <= 10000)
                {
                    percent = 0.48;
                    minCoef = 3.5;
                }
                else if (priceEUR <= 32000)
                {
                    percent = 0.48;
                    minCoef = 5.5;
                }
                else if (priceEUR <= 84500)
                {
                    percent = 0.48;
                    minCoef = 7.5;
                }
                else if (priceEUR <= 169000)
                {
                    percent = 0.48;
                    minCoef = 15;
                }
                else if (priceEUR > 169000)
                {
                    percent = 0.48;
                    minCoef = 20;
                }
            }
            //От 3 до 5 лет:
            else if (age > 3 && age <= 5)
            {
                if (engine <= 1000)
                    minCoef = 1.5;
                else if (engine <= 1500)
                    minCoef = 1.7;
                else if (engine <= 1800)
                    minCoef = 2.5;
                else if (engine <= 2300)
                    minCoef = 2.7;
                else if (engine <= 3000)
                    minCoef = 3;
                else if (engine > 3000)
                    minCoef = 3.6;
            }
            //Больше 5 лет:
            else if (age > 5)
            {
                if (engine <= 1000)
                    minCoef = 3;
                else if (engine <= 1500)
                    minCoef = 3.2;
                else if (engine <= 1800)
                    minCoef = 3.5;
                else if (engine <= 2300)
                    minCoef = 4.8;
                else if (engine <= 3000)
                    minCoef = 5;
                else if (engine > 3000)
                    minCoef = 5.7;
            }

            if (!isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.23;
                        minCoef = 0.67;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.73;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.23;
                        minCoef = 0.83;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1000)
                        minCoef = 1.4;
                    else if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 1800)
                        minCoef = 1.6;
                    else if (engine <= 2300)
                        minCoef = 2.2;
                    else if (engine <= 3000)
                        minCoef = 3.2;
                    else if (engine > 3000)
                        minCoef = 3.2;
                }
            }

            else
            if (isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.8;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.4;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 2500)
                        minCoef = 2.2;
                    else if (engine > 2500)
                        minCoef = 3.2;
                }

            }


            //Вычисляем таможенную ставку (в евро):
            fee = priceEUR * percent;
            minimumFee = engine * minCoef;

            //Если ставка ниже минимально разрешённого порога, принимаем порог:
            if (fee < minimumFee)
                fee = minimumFee;

            //Переводим Грн в евро:
            fee = fee * 32;

            return Convert.ToInt32(fee);
        }

        //Акциз:
        private int Calc2_Excise(int engineCapacity)
        {
            int excise = 0;    //Акциз
            int priceCoef = 0; //Коэффициент Грн\объем двигателя

            //Задаём коэффициент на основании объема двигателя:
            if (engineCapacity <= 900)
                priceCoef = 0;
            else if (engineCapacity <= 1500)
                priceCoef = 3;
            else if (engineCapacity <= 2000)
                priceCoef = 7;
            else if (engineCapacity <= 3000)
                priceCoef = 9;
            else if (engineCapacity <= 4000)
                priceCoef = 12;
            else if (engineCapacity <= 5000)
                priceCoef = 16;
            else if (engineCapacity > 5000)
                priceCoef = 25;

            //Рассчитываем акциз:
            excise = engineCapacity * priceCoef;

            return excise;
        }

        //НДС:
        private int Calc2_NDS(int price, int mainTax, int excise)
        {
            // НДС (20%) расчитывается от суммы: стоимость авто + таможенная пошлина + акциз.
            double NDS;

            NDS = (price + mainTax + excise) * 0.20;

            return Convert.ToInt32(NDS);
        }

      

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox13.Text == "Грн.")
                numericUpDown3.Text = "1";
            else if (comboBox13.Text == "Евро")
                numericUpDown3.Text = EUR.ToString();
            else if (comboBox13.Text == "Долл.")
                numericUpDown3.Text = USD.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = true;
            button3.Visible = false;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value > 32)
                numericUpDown3.Value = 32;

            if (comboBox13.Text == "Евро")
                EUR = (double)numericUpDown3.Value;
            else if (comboBox13.Text == "Долл.")
                USD = (double)numericUpDown3.Value;
        }

        //----------------------------------------
       
        //-----------------расчет плитки 3 (мото)----------------
        private void button7_Click(object sender, EventArgs e)
        {

            //0. Если пользователь забыл ввести данные:
            if (this.textBox7.Text == "" || textBox8.Text == "" || comboBox15.Text == "" || comboBox16.Text == "" || comboBox17.Text == "" )
            {
                //Выводим ошибку
                MessageBox.Show("Данные не могут быть считаны. \n\nЗаполните пожалуйста все поля.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //Останавливаем работу функции
            }

            int age = 0;    // Возраст автомобиля

            int Box3 = 0; // Объём двигателя
            bool isDiesel = false;  // Дизельный двигатель?
            int priceUser = 0;      // Стоимость автомобиля в валюте пользователя
            int priceGRN = 0;       // Стоимость автомобиля в GRN
            string currency = "Грн";// Выбранная пользователем валюта

            //_____1. Считывание данных с формы-плитки № 2:
      
       
            // Задаём выбранную пользователем валюту (currency):
            if (comboBox15.Text == "Грн.")
                currency = "Грн";
            else if (comboBox15.Text == "Евро")
            {
                currency = "EUR";
                EUR = Convert.ToDouble(numericUpDown4.Value);
            }
            else if (comboBox15.Text == "Долл.")
            {
                currency = "USD";
                USD = Convert.ToDouble(numericUpDown4.Value);
            }

            // Устанавливаем остальные параметры:

            Box3 = Convert.ToInt32(this.textBox7.Text);

            if (comboBox16.Text == "бензиновый")
                isDiesel = true;

            // Считываем цену в валюте пользователя:
            priceUser = Convert.ToInt32(textBox8.Text);
            // Переводим валюту пользователя в Грн:
            if (currency == "Грн")
                priceGRN = priceUser;
            else if (currency == "EUR")
                priceGRN = Convert.ToInt32(priceUser * EUR);
            else if (currency == "USD")
                priceGRN = Convert.ToInt32(priceUser * USD);

            // (все принимают на вход сумму в гривнях)

            //_                Расчёт данных:

            int mainTax = Calc3_MainTax(age, Box3, isDiesel, priceGRN); // Таможенная пошлина


            int excise = 0;
            int NDS = 0;

            //Рассчитываем акциз и НДС:
            excise = Calc3_Excise(Box3);
            NDS = Calc3_NDS(priceGRN, mainTax, excise);
            //  }

            //______3. Вартість авто із розмитненням:
            labelTaxResult.Text = String.Format("{0:N0}", mainTax) + " Грн.";

            labelExciseResult.Text = String.Format("{0:N0}", excise) + " Грн.";

            labelNDSResult.Text = String.Format("{0:N0}", NDS) + " Грн.";

            //Расчёт итоговой стоимости:
            int finalresult = mainTax + excise + NDS;
            labelFinalResult.Text = String.Format("{0:N0}", finalresult) + " Грн.";
        }



        //Основная пошлина:
        private int Calc3_MainTax(int age, int engine, bool isDiesel, int priceGRN)
        {
            double fee = 0; //Пошлина
            double minimumFee; //Минимальный размер пошлины для рассматриваемого авто

            int priceEUR = priceGRN / 32; // Цена в евро

            double percent = 0; //Процент от стоимости авто
            double minCoef = 0; //Минимальный коэффициент евро/см (относительно объёма двигателя)


            //Меньше 3 лет:
            if (age <= 3)
            {
                if (priceEUR <= 5500)
                {
                    percent = 0.54;
                    minCoef = 2.5;
                }
                else if (priceEUR <= 10000)
                {
                    percent = 0.48;
                    minCoef = 3.5;
                }
                else if (priceEUR <= 32000)
                {
                    percent = 0.48;
                    minCoef = 5.5;
                }
                else if (priceEUR <= 84500)
                {
                    percent = 0.48;
                    minCoef = 7.5;
                }
                else if (priceEUR <= 169000)
                {
                    percent = 0.48;
                    minCoef = 15;
                }
                else if (priceEUR > 169000)
                {
                    percent = 0.48;
                    minCoef = 20;
                }
            }
            //От 3 до 5 лет:
            else if (age > 3 && age <= 5)
            {
                if (engine <= 1000)
                    minCoef = 1.5;
                else if (engine <= 1500)
                    minCoef = 1.7;
                else if (engine <= 1800)
                    minCoef = 2.5;
                else if (engine <= 2300)
                    minCoef = 2.7;
                else if (engine <= 3000)
                    minCoef = 3;
                else if (engine > 3000)
                    minCoef = 3.6;
            }
            //Больше 5 лет:
            else if (age > 5)
            {
                if (engine <= 1000)
                    minCoef = 3;
                else if (engine <= 1500)
                    minCoef = 3.2;
                else if (engine <= 1800)
                    minCoef = 3.5;
                else if (engine <= 2300)
                    minCoef = 4.8;
                else if (engine <= 3000)
                    minCoef = 5;
                else if (engine > 3000)
                    minCoef = 5.7;
            }

            if (!isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.23;
                        minCoef = 0.67;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.73;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.23;
                        minCoef = 0.83;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1000)
                        minCoef = 1.4;
                    else if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 1800)
                        minCoef = 1.6;
                    else if (engine <= 2300)
                        minCoef = 2.2;
                    else if (engine <= 3000)
                        minCoef = 3.2;
                    else if (engine > 3000)
                        minCoef = 3.2;
                }
            }

            else
            if (isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.8;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.4;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 2500)
                        minCoef = 2.2;
                    else if (engine > 2500)
                        minCoef = 3.2;
                }

            }


            //Вычисляем таможенную ставку (в евро):
            fee = priceEUR * percent;
            minimumFee = engine * minCoef;

            //Если ставка ниже минимально разрешённого порога, принимаем порог:
            if (fee < minimumFee)
                fee = minimumFee;

            //Переводим Грн в евро:
            fee = fee * 32;

            return Convert.ToInt32(fee);
        }

        //Акциз:
        private int Calc3_Excise(int engineCapacity)
        {
            int excise = 0;    //Акциз
            int priceCoef = 0; //Коэффициент Грн\объем двигателя

            //Задаём коэффициент на основании объема двигателя:
            if (engineCapacity <= 900)
                priceCoef = 0;
            else if (engineCapacity <= 1500)
                priceCoef = 3;
            else if (engineCapacity <= 2000)
                priceCoef = 7;
            else if (engineCapacity <= 3000)
                priceCoef = 9;
            else if (engineCapacity <= 4000)
                priceCoef = 12;
            else if (engineCapacity <= 5000)
                priceCoef = 16;
            else if (engineCapacity > 5000)
                priceCoef = 25;

            //Рассчитываем акциз:
            excise = engineCapacity * priceCoef;

            return excise;
        }

        //НДС:
        private int Calc3_NDS(int price, int mainTax, int excise)
        {
            // НДС (20%) расчитывается от суммы: стоимость авто + таможенная пошлина + акциз.
            double NDS;

            NDS = (price + mainTax + excise) * 0.20;

            return Convert.ToInt32(NDS);
        }



        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox15.Text == "Грн.")
                numericUpDown4.Text = "1";
            else if (comboBox15.Text == "Евро")
                numericUpDown4.Text = EUR.ToString();
            else if (comboBox15.Text == "Долл.")
                numericUpDown4.Text = USD.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            numericUpDown4.Enabled = true;
            button6.Visible = false;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value > 32)
                numericUpDown3.Value = 32;

            if (comboBox15.Text == "Евро")
                EUR = (double)numericUpDown4.Value;
            else if (comboBox13.Text == "Долл.")
                USD = (double)numericUpDown4.Value;
        }

        //----------------------------------------
        
        //-----------------расчет плитки 4 ( автобусы )----------------

        private void button5_Click(object sender, EventArgs e)
        {

            //0. Если пользователь забыл ввести данные:
            if (this.textBox5.Text == "" || textBox4.Text == "" || comboBox5.Text == "" || comboBox6.Text == "" || comboBox7.Text == "" || comboBox8.Text == "")
            {
                //Выводим ошибку
                MessageBox.Show("Данные не могут быть считаны. \n\nЗаполните пожалуйста все поля.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //Останавливаем работу функции
            }

            int age = 0;    // Возраст автомобиля

            int Box3 = 0; // Объём двигателя
            bool isDiesel = false;  // Дизельный двигатель?
            int priceUser = 0;      // Стоимость автомобиля в валюте пользователя
            int priceGRN = 0;       // Стоимость автомобиля в GRN
            string currency = "Грн";// Выбранная пользователем валюта

            //_____1. Считывание данных с формы-плитки № 1:

            // Задаём возраст автомобиля (age):
            if (comboBox7.Text == "До 3 лет")
                age = 0;
            else if (comboBox7.Text == "От 3 до 5 лет")
                age = 3;
            else if (comboBox7.Text == "От 5 до 7 лет")
                age = 6;
            else if (comboBox7.Text == "Более 15 лет")
                age = 10;

            // Задаём выбранную пользователем валюту (currency):
            if (comboBox8.Text == "Грн.")
                currency = "Грн";
            else if (comboBox8.Text == "Евро")
            {
                currency = "EUR";
                EUR = Convert.ToDouble(numericUpDown2.Value);
            }
            else if (comboBox8.Text == "Долл.")
            {
                currency = "USD";
                USD = Convert.ToDouble(numericUpDown2.Value);
            }

            // Устанавливаем остальные параметры:

            Box3 = Convert.ToInt32(this.textBox5.Text);

            if (comboBox6.Text == "Дизельный")
                isDiesel = true;

            // Считываем цену в валюте пользователя:
            priceUser = Convert.ToInt32(textBox4.Text);
            // Переводим валюту пользователя в Грн:
            if (currency == "Грн")
                priceGRN = priceUser;
            else if (currency == "EUR")
                priceGRN = Convert.ToInt32(priceUser * EUR);
            else if (currency == "USD")
                priceGRN = Convert.ToInt32(priceUser * USD);

            // (все принимают на вход сумму в гривнях)

            //_                Расчёт данных:

            int mainTax = Calc_MainTax(age, Box3, isDiesel, priceGRN); // Таможенная пошлина
            int excise = 0;
            int NDS = 0;
           
            //Рассчитываем акциз и НДС:
            excise = Calc_Excise(Box3);
            NDS = Calc_NDS(priceGRN, mainTax, excise);
           

            //______3. Вартість авто із розмитненням:
            labelTaxResult.Text = String.Format("{0:N0}", mainTax) + " Грн.";

            labelExciseResult.Text = String.Format("{0:N0}", excise) + " Грн.";

            labelNDSResult.Text = String.Format("{0:N0}", NDS) + " Грн.";

            //Расчёт итоговой стоимости:
            int finalresult = mainTax + excise + NDS;
            labelFinalResult.Text = String.Format("{0:N0}", finalresult) + " Грн.";
        }

        //Основная пошлина:
        private int Calc1_MainTax(int age, int engine, bool isDiesel, int priceGRN)
        {
            double fee = 0; //Пошлина
            double minimumFee; //Минимальный размер пошлины для рассматриваемого авто

            int priceEUR = priceGRN / 32; // Цена в евро

            double percent = 0; //Процент от стоимости авто
            double minCoef = 0; //Минимальный коэффициент евро/см (относительно объёма двигателя)


            //Меньше 3 лет:
            if (age <= 3)
            {
                if (priceEUR <= 5500)
                {
                    percent = 0.54;
                    minCoef = 2.5;
                }
                else if (priceEUR <= 10000)
                {
                    percent = 0.48;
                    minCoef = 3.5;
                }
                else if (priceEUR <= 32000)
                {
                    percent = 0.48;
                    minCoef = 5.5;
                }
                else if (priceEUR <= 84500)
                {
                    percent = 0.48;
                    minCoef = 7.5;
                }
                else if (priceEUR <= 169000)
                {
                    percent = 0.48;
                    minCoef = 15;
                }
                else if (priceEUR > 169000)
                {
                    percent = 0.48;
                    minCoef = 20;
                }
            }
            //От 3 до 5 лет:
            else if (age > 3 && age <= 5)
            {
                if (engine <= 1000)
                    minCoef = 1.5;
                else if (engine <= 1500)
                    minCoef = 1.7;
                else if (engine <= 1800)
                    minCoef = 2.5;
                else if (engine <= 2300)
                    minCoef = 2.7;
                else if (engine <= 3000)
                    minCoef = 3;
                else if (engine > 3000)
                    minCoef = 3.6;
            }
            //Больше 5 лет:
            else if (age > 5)
            {
                if (engine <= 1000)
                    minCoef = 3;
                else if (engine <= 1500)
                    minCoef = 3.2;
                else if (engine <= 1800)
                    minCoef = 3.5;
                else if (engine <= 2300)
                    minCoef = 4.8;
                else if (engine <= 3000)
                    minCoef = 5;
                else if (engine > 3000)
                    minCoef = 5.7;
            }

            if (!isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.23;
                        minCoef = 0.67;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.73;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.23;
                        minCoef = 0.83;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1000)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine <= 1800)
                    {
                        percent = 0.25;
                        minCoef = 0.45;
                    }
                    else if (engine <= 2300)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine <= 3000)
                    {
                        percent = 0.25;
                        minCoef = 0.55;
                    }
                    else if (engine > 3000)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1000)
                        minCoef = 1.4;
                    else if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 1800)
                        minCoef = 1.6;
                    else if (engine <= 2300)
                        minCoef = 2.2;
                    else if (engine <= 3000)
                        minCoef = 3.2;
                    else if (engine > 3000)
                        minCoef = 3.2;
                }
            }

            else
            if (isDiesel)
            {
                //Меньше 3 лет:
                if (age <= 3)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.23;
                        minCoef = 0.8;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.2;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.23;
                        minCoef = 1.57;
                    }
                }
                //От 3 до 7 лет:
                else if (age > 3 && age <= 7)
                {
                    if (engine <= 1500)
                    {
                        percent = 0.25;
                        minCoef = 0.4;
                    }
                    else if (engine <= 2500)
                    {
                        percent = 0.25;
                        minCoef = 0.5;
                    }
                    else if (engine > 2500)
                    {
                        percent = 0.25;
                        minCoef = 1;
                    }
                }
                //Старше 7 лет:
                else if (age > 7)
                {
                    if (engine <= 1500)
                        minCoef = 1.5;
                    else if (engine <= 2500)
                        minCoef = 2.2;
                    else if (engine > 2500)
                        minCoef = 3.2;
                }

            }


            //Вычисляем таможенную ставку (в евро):
            fee = priceEUR * percent;
            minimumFee = engine * minCoef;

            //Если ставка ниже минимально разрешённого порога, принимаем порог:
            if (fee < minimumFee)
                fee = minimumFee;

            //Переводим Грн в евро:
            fee = fee * 32;

            return Convert.ToInt32(fee);
        }

        //Акциз:
        private int Calc1_Excise(int engineCapacity)
        {
            int excise = 0;    //Акциз
            int priceCoef = 0; //Коэффициент Грн\объем двигателя

            //Задаём коэффициент на основании объема двигателя:
            if (engineCapacity <= 900)
                priceCoef = 0;
            else if (engineCapacity <= 1500)
                priceCoef = 3;
            else if (engineCapacity <= 2000)
                priceCoef = 7;
            else if (engineCapacity <= 3000)
                priceCoef = 9;
            else if (engineCapacity <= 4000)
                priceCoef = 12;
            else if (engineCapacity <= 5000)
                priceCoef = 16;
            else if (engineCapacity > 5000)
                priceCoef = 25;

            //Рассчитываем акциз:
            excise = engineCapacity * priceCoef;

            return excise;
        }

        //НДС:
        private int Calc1_NDS(int price, int mainTax, int excise)
        {
            // НДС (20%) расчитывается от суммы: стоимость авто + таможенная пошлина + акциз.
            double NDS;

            NDS = (price + mainTax + excise) * 0.20;

            return Convert.ToInt32(NDS);
        }

      

        //Функция, проверяющая напечатанный символ:
        private void HandleKey1_Number(KeyPressEventArgs e)
        {
            //Если клавиша - это цифра или backspace:
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                #pragma warning disable CS0642 // Возможно, ошибочный пустой оператор
                ;
            #pragma warning restore CS0642 // Возможно, ошибочный пустой оператор
            // Разрешаем напечатать символ

            //Если мы нажали не цифру, то запрещаем печатать символ:
            else e.Handled = true;
        }

        //Функции, ловящие событие "печать символа" (KeyPress):
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);

        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);
        }
        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKey_Number(e);
        }

       


        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.Text == "Грн.")
                numericUpDown2.Text = "1";
            else if (comboBox8.Text == "Евро")
                numericUpDown2.Text = EUR.ToString();
            else if (comboBox8.Text == "Долл.")
                numericUpDown2.Text = USD.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = true;
            button1.Visible = false;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > 32)
                numericUpDown2.Value = 32;

            if (comboBox8.Text == "Евро")
                EUR = (double)numericUpDown2.Value;
            else if (comboBox8.Text == "Долл.")
                USD = (double)numericUpDown1.Value;
        }
    
        //--------------------------------------------------------------------------------------------------------------------------  
        private void tableLayoutTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
           
        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox4_MouseEnter_1(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.Red;
        }

        private void pictureBox4_MouseLeave_1(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.White;
        }

        private void pictureBox4_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage3);
            
        }
        
        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage1);
            foreach (TabPage page in tabControl1.TabPages)  //метод очистки
            {
                ClearText(page);
            }

        }
        private void pictureBox2_MouseLeave_1(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.White;
        }

        private void pictureBox2_MouseEnter_1(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.Red;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);
            foreach (TabPage page in tabControl1.TabPages)  //метод очистки
            {
                ClearText(page);
            }
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.White;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        { 
            tabControl1.SelectTab(tabPage4);
           
           
              foreach (TabPage page in tabControl1.TabPages)  //метод очистки
              {
                  ClearText(page);
              }

        }//------------очистка плиток-------------
       
          static void ClearText(Control parent)
        {
            if (parent is TextBox || parent is ComboBox)
            {
                parent.Text = "";
            }
            else
            {
                foreach (Control ct in parent.Controls)
                {
                    ClearText(ct);
                }
            }
        }
        //-------------------
        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.Red;
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.White;
        }

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://help.ria.com/uk/index.php?/Knowledgebase/Article/View/630/146#start_content");
        }

        private void pictureBox7_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://help.ria.com/uk/index.php?/Knowledgebase/Article/View/631/146/primery-rschet-stoimosti-rstmozhki-rznykh-tipov-trnsportnykh-sredstv");
        }

        private void pictureBox8_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://auto.ria.com/uk/news/tag/rastamozhka/");
        }
        //кнопка корректировки курса
        private void button1_Click_1(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            button1.Visible = false;
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            button5_Click(sender, e);


        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }
      
       
       

        private void button7_Click_1(object sender, EventArgs e)
        {
            button7_Click(sender, e);
        }
        //кнопка корректировки курса грузовых
        private void button3_Click_2(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = true;
            button3.Visible = false;
        }
        //кнопка корректировки курса мото
        private void button6_Click_2(object sender, EventArgs e)
        {
            numericUpDown4.Enabled = true;
            button6.Visible = false;
        }
        //кнопка корректировки курса автобусов
        private void button2_Click_2(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = true;
            button2.Visible = false;
        }
    }
}