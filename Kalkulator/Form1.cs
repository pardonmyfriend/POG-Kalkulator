using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kalkulator
{
    public enum Operation
    {
        None,
        Addition,
        Substraction,
        Multiplication,
        Division
    }

    public partial class Form1 : Form
    {
        private string firstValue;
        private string secondValue = "";
        private Operation currentOperation = Operation.None;
        private bool resultOnScreen = false;
        private bool zeroDivision = false;
        private bool negOn = false;
        public Form1()
        {
            InitializeComponent();
            mainBox.Text = "0";
            button_b.Text = char.ConvertFromUtf32(0x2190);
        }

        private void OperationButtonState(bool value)
        {
            button_plus.Enabled = value;
            button_minus.Enabled = value;
            button_mult.Enabled = value;
            button_div.Enabled = value;
            button_b.Enabled = value;
        }

        private void ResultButtonState(bool value)
        {
            button_calc.Enabled = value;
        }

        private void buttonNumberClick(object sender, EventArgs e)
        {
            ResultButtonState(true);

            string number = (sender as System.Windows.Forms.Button).Text;

            if (currentOperation != Operation.None && secondValue == "")
                   mainBox.Text = string.Empty;

            if (zeroDivision)
            {
                zeroDivision = false;
                smallBox.Text = string.Empty;
            }

            if (resultOnScreen)
            {
                resultOnScreen = false;
                mainBox.Text = string.Empty;
            }

            if (mainBox.Text == "0")
                mainBox.Text = string.Empty;

            if (mainBox.Text.Length < 13)
            {
                if (currentOperation != Operation.None)
                {
                    if (secondValue == "")
                        mainBox.Text = string.Empty;
                    secondValue += number;
                    button_b.Enabled = true;
                }
                else
                    OperationButtonState(true);

                mainBox.Text += number;
            }     
        }

        private void buttonOperationClick(object sender, EventArgs e)
        {
            ResultButtonState(false);
            button_separator.Enabled = true;

            if (resultOnScreen)
                resultOnScreen = false;

            if (currentOperation != Operation.None && secondValue != "")
            {
                double firstNumber = 0;
                double secondNumber = 0;

                if (Double.TryParse(firstValue, out firstNumber) && Double.TryParse(secondValue, out secondNumber))
                {
                    var result = Calculate(firstNumber, secondNumber);

                    if (!zeroDivision)
                    {
                        string result_s = result.ToString();

                        if (result_s.Length > 13)
                        {
                            if (result > 9999999999999)
                                result_s = String.Format("{0,13:E}", result);
                            else
                            {
                                for (int i = 0; i < result_s.Length; i++)
                                {
                                    if (i == 13)
                                    {
                                        int n = result_s[i - 1] - '0';
                                        int m = result_s[i] - '0';
                                        if (m > 4)
                                            n++;
                                        StringBuilder sb = new StringBuilder(result_s);
                                        sb[i - 1] = Convert.ToChar(n + '0');
                                        result_s = sb.ToString();
                                        result_s = result_s.Substring(0, 13);
                                        break;
                                    }
                                }
                            }
                        }

                        smallBox.Text += result_s;
                        mainBox.Text = result_s;
                        firstValue = result_s;

                        OperationButtonState(true);
                        ResultButtonState(true);
                        button_separator.Enabled = true;
                    }

                    zeroDivision = false;
                    secondValue = string.Empty;
                    resultOnScreen = true;
                    currentOperation = Operation.None;
                    button_b.Enabled = false;
                }
            }
            else
                firstValue = mainBox.Text;

            string operation = (sender as System.Windows.Forms.Button).Text;

            smallBox.Text = mainBox.Text;
            smallBox.Text += operation;

            switch (operation)
            {
                case "+":
                    currentOperation = Operation.Addition;
                    break;
                case "-":
                    currentOperation = Operation.Substraction;
                    break;
                case "*":
                    currentOperation = Operation.Multiplication;
                    break;
                case "/":
                    currentOperation = Operation.Division;
                    break;
                default:
                    currentOperation = Operation.None;
                    break;
            }
        }

        private double Calculate(double firstNumber, double secondNumber)
        {
            switch (currentOperation)
            {
                case Operation.None:
                    return firstNumber;
                case Operation.Addition:
                    return firstNumber + secondNumber;
                case Operation.Substraction:
                    return firstNumber - secondNumber;
                case Operation.Multiplication:
                    return firstNumber * secondNumber;
                case Operation.Division:
                    if (secondNumber == 0)
                    {
                        zeroDivision = true;
                        OperationButtonState(false);
                        mainBox.Text = "Nie dziel przez 0!";
                        return 0;
                    }
                    return firstNumber / secondNumber;
            }
            return 0;
        }

        private void buttonResultClick(object sender, EventArgs e)
        {
            double firstNumber = 0;
            double secondNumber = 0;

            if (Double.TryParse(firstValue, out firstNumber) && Double.TryParse(secondValue, out secondNumber))
            {
                var result = Calculate(firstNumber, secondNumber);

                if(!zeroDivision)
                {

                    string result_s = result.ToString();

                    if (result_s.Length > 13)
                    {
                        if (result > 9999999999999)
                            result_s = String.Format("{0,13:E}", result);
                        else
                        {
                            for (int i = 0; i < result_s.Length; i++)
                            {
                                if (i == 13)
                                {
                                    int n = result_s[i - 1] - '0';
                                    int m = result_s[i] - '0';
                                    if (m > 4)
                                        n++;
                                    StringBuilder sb = new StringBuilder(result_s);
                                    sb[i - 1] = Convert.ToChar(n + '0');
                                    result_s = sb.ToString();
                                    result_s = result_s.Substring(0, 13);
                                    break;
                                }
                            }
                        }
                    }

                    smallBox.Text += mainBox.Text;
                    smallBox.Text += "=";
                    mainBox.Text = result_s;

                    OperationButtonState(true);
                    ResultButtonState(true);
                    button_separator.Enabled = true;
                }

                secondValue = string.Empty;
                resultOnScreen = true;
                currentOperation = Operation.None;
                button_b.Enabled = false;
            }
        }

        private void buttonClearClick(object sender, EventArgs e)
        {
            mainBox.Text = "0";
            smallBox.Text = string.Empty;
            firstValue = string.Empty;
            secondValue = string.Empty;
            currentOperation = Operation.None;
        }

        private void buttonCEClick(object sender, EventArgs e)
        {
            mainBox.Text = "0";
            if (currentOperation != Operation.None)
                secondValue = string.Empty;
            else
            {
                smallBox.Text = string.Empty;
                firstValue = string.Empty;
                secondValue = string.Empty;
            }
        }

        private void buttonBackspaceClick(object sender, EventArgs e)
        {
            int textLength = mainBox.Text.Length;
            if (textLength > 1 && mainBox.Text != "0")
            {
                mainBox.Text = mainBox.Text.Substring(0, textLength - 1);
                if (currentOperation != Operation.None)
                    secondValue = secondValue.Substring(0, textLength - 1);
            }
            else if (textLength == 1 && mainBox.Text != "0")
            {
                mainBox.Text = "0";
                if (currentOperation != Operation.None)
                    secondValue = secondValue.Substring(0, textLength - 1);
            }
        }

        private void buttonNegClick(object sender, EventArgs e)
        {
            if (mainBox.Text != "0")
            {
                if (negOn)
                {
                    negOn = false;
                    mainBox.Text = mainBox.Text.Substring(1);
                    if (currentOperation != Operation.None)
                        secondValue = secondValue.Substring(1);
                    else
                        firstValue = firstValue.Substring(1);
                }
                else
                {
                    negOn = true;
                    mainBox.Text = "-" + mainBox.Text;
                    if (currentOperation != Operation.None)
                        secondValue = "-" + secondValue;
                    else
                        firstValue = "-" + firstValue;
                }
            }
        }

        private void buttonSeparatorClick(object sender, EventArgs e)
        {
            button_separator.Enabled = false;

            string sep = ",";

            if (resultOnScreen)
            {
                resultOnScreen = false;
                mainBox.Text = string.Empty;
                sep = "0,";
            }

            ResultButtonState(true);

            if (currentOperation != Operation.None)
            {
                if (secondValue == "")
                {
                    sep = "0,";
                    mainBox.Text = string.Empty;
                }
                secondValue += sep;
                button_b.Enabled = true;
            }
            else
                OperationButtonState(true);

            if (mainBox.Text.Length < 13)
                mainBox.Text += sep;
        }
    }
}
