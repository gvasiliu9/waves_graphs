using System;
using System.Timers;
using System.Windows.Input;
using WavesGraphs.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WavesGraphs.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimePicker : ContentView
    {
        #region Properties & Fields

        private DateTime _time;

        private bool _runTimer;

        private int _buttonPressTimerPeriod = 100;

        private int _buttonPressTimerDuration;

        private Timer _countDownTimer;

        private uint _countDownPeriod = 1 * 60000; // 1 minute = 60000 ms

        private enum TimerDirection
        {
            Decrement,
            Increment
        }

        #region Bindable Properties

        public static readonly BindableProperty DefaultHoursProperty = BindableProperty
            .Create(nameof(DefaultHours), typeof(int), typeof(TimePicker), default(int));

        public int DefaultHours
        {
            get
            {
                return (int)GetValue(DefaultHoursProperty);
            }
            set
            {
                SetValue(DefaultHoursProperty, value);
            }
        }

        public static readonly BindableProperty DefaultMinutesProperty = BindableProperty
            .Create(nameof(DefaultMinutes), typeof(int), typeof(TimePicker), default(int));

        public int DefaultMinutes
        {
            get
            {
                return (int)GetValue(DefaultMinutesProperty);
            }
            set
            {
                SetValue(DefaultMinutesProperty, value);
            }
        }

        public static readonly BindableProperty HoursProperty = BindableProperty
            .Create(nameof(Hours), typeof(int), typeof(TimePicker), default(int));

        public int Hours
        {
            get
            {
                return (int)GetValue(HoursProperty);
            }
            set
            {
                SetValue(HoursProperty, value);
            }
        }

        public static readonly BindableProperty MinutesProperty = BindableProperty
            .Create(nameof(Minutes), typeof(int), typeof(TimePicker), default(int));

        public int Minutes
        {
            get
            {
                return (int)GetValue(MinutesProperty);
            }
            set
            {
                SetValue(MinutesProperty, value);
            }
        }

        public static readonly BindableProperty MaxHoursProperty = BindableProperty
            .Create(nameof(MaxHours), typeof(int), typeof(TimePicker), default(int));

        public int MaxHours
        {
            get
            {
                return (int)GetValue(MaxHoursProperty);
            }
            set
            {
                SetValue(MaxHoursProperty, value);
            }
        }

        public static readonly BindableProperty StepProperty = BindableProperty
            .Create(nameof(Step), typeof(int), typeof(TimePicker), default(int));

        public int Step
        {
            get
            {
                return (int)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly BindableProperty StartTextProperty = BindableProperty
            .Create(nameof(StartText), typeof(string), typeof(TimePicker), default(string));

        public string StartText
        {
            get
            {
                return (string)GetValue(StartTextProperty);
            }
            set
            {
                SetValue(StartTextProperty, value);
            }
        }

        public static readonly BindableProperty StopTextProperty = BindableProperty
            .Create(nameof(StopText), typeof(string), typeof(TimePicker), default(string));

        public string StopText
        {
            get
            {
                return (string)GetValue(StopTextProperty);
            }
            set
            {
                SetValue(StopTextProperty, value);
            }
        }

        public static readonly BindableProperty IsStartedProperty = BindableProperty
            .Create(nameof(IsStarted), typeof(bool), typeof(TimePicker), default(bool));

        public bool IsStarted
        {
            get
            {
                return (bool)GetValue(IsStartedProperty);
            }
            set
            {
                SetValue(IsStartedProperty, value);
            }
        }

        public static readonly BindableProperty CountDownColorProperty = BindableProperty
            .Create(nameof(CountDownColor), typeof(Color), typeof(FanBoostBar), default(Color));

        public Color CountDownColor
        {
            get
            {
                return (Color)GetValue(CountDownColorProperty);
            }

            set
            {
                SetValue(CountDownColorProperty, value);
            }
        }

        // Commands
        public static readonly BindableProperty StartCommandProperty = BindableProperty
            .Create(nameof(Command), typeof(ICommand), typeof(TimePicker), null);

        public ICommand StartCommand
        {
            get { return (ICommand)GetValue(StartCommandProperty); }
            set { SetValue(StartCommandProperty, value); }
        }

        public static readonly BindableProperty StopCommandProperty = BindableProperty
            .Create(nameof(Command), typeof(ICommand), typeof(TimePicker), null);

        public ICommand StopCommand
        {
            get { return (ICommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }

        #endregion 

        #endregion

        public TimePicker()
        {
            InitializeComponent();
        }

        #region Methods

        private void StopButtonPressTimer()
            => _runTimer = false;

        private void DecrementTime(int step)
        {
            if (_time.Hour > 0 || _time.Minute > 0)
                UpdateLabelText(_time.AddMinutes(-step));
        }

        private void IncrementTime(int step)
        {
            if (_time.Hour < MaxHours)
                UpdateLabelText(_time.AddMinutes(step));
        }

        private void UpdateLabelText(DateTime pickerTime)
        {
            _time = pickerTime;

            label.Opacity = 0;

            if (_time.Hour > 0 && _time.Minute > 0)
            {
                label.Text = $"{pickerTime.Hour} h {pickerTime.Minute} min";
            }
            else if (_time.Hour > 0 && _time.Hour > 1)
            {
                label.Text = $"{pickerTime.Hour} hours";
            }
            else if (_time.Hour == 1)
            {
                label.Text = $"{pickerTime.Hour} hour";
            }
            else if (_time.Minute > 1)
            {
                label.Text = $"{pickerTime.Minute} minutes";
            }
            else if (_time.Minute == 1)
            {
                label.Text = $"{pickerTime.Minute} minute";
            }

            label.Opacity = 1;
        }

        /// <summary>
        /// Used to increase/decrease time picker value
        /// </summary>
        /// <param name="period"></param>
        /// <param name="timerDirection"></param>
        private void RunButtonPressTimer(int period, TimerDirection timerDirection)
        {
            _runTimer = true;

            _buttonPressTimerDuration = 0;

            Device.StartTimer(TimeSpan.FromMilliseconds(period), () =>
            {
                if (_buttonPressTimerDuration > 3000 && timerDirection == TimerDirection.Decrement)
                {
                    UpdateLabelText(DateTime.MinValue.AddMinutes(Step));

                    return false;
                }
                else if (_buttonPressTimerDuration > 3000 && timerDirection == TimerDirection.Increment)
                {
                    UpdateLabelText(DateTime.MinValue.AddHours(MaxHours));

                    return false;
                }
                else if (timerDirection == TimerDirection.Decrement)
                {
                    DecrementTime(Step);
                }
                else if (timerDirection == TimerDirection.Increment)
                {
                    IncrementTime(Step);
                }

                _buttonPressTimerDuration += _buttonPressTimerPeriod;

                return _runTimer;
            });
        }

        /// <summary>
        /// Run timer, with specified period in minutes
        /// </summary>
        /// <param name="period"></param>
        private void StartCountDown()
        {
            countDownIndicator.Opacity = 1;

            StopCountDownAnimation();

            StartCountDownAnimation(_countDownPeriod);

            _countDownTimer = new Timer(_countDownPeriod);
            _countDownTimer.Enabled = true;
            _countDownTimer.AutoReset = true;
            _countDownTimer.Elapsed += CountDownTimer_Elapsed;
        }

        private void StopCountDown()
        {
            _countDownTimer.Stop();

            StopCountDownAnimation();
        }

        private void CountDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var minutes = (int)(_countDownPeriod / 60000);

                if (_time.Hour == 0 && _time.Minute == 1)
                {
                    IsStarted = false;
                    return;
                }

                DecrementTime(minutes);

                // Animate count down
                StartCountDownAnimation(_countDownPeriod);
            });
        }

        private void StartCountDownAnimation(uint lengthInMiliseconds = 250)
        {
            countDownIndicator.Opacity = 1;

            var animation = new Animation(w => countDownIndicator.WidthRequest = w, timePicker.Width);
            animation.Commit(countDownIndicator, "countDownProgressAnimation", 16, lengthInMiliseconds);
        }

        private void StopCountDownAnimation()
        {
            countDownIndicator.Opacity = 0;

            countDownIndicator.AbortAnimation("countDownProgressAnimation");
        }

        #region Events

        private void DecrementButton_Pressed(object sender, EventArgs e)
            => RunButtonPressTimer(_buttonPressTimerPeriod, TimerDirection.Decrement);

        private void IncrementButton_Pressed(object sender, EventArgs e)
             => RunButtonPressTimer(_buttonPressTimerPeriod, TimerDirection.Increment);

        private void DecrementButton_Released(object sender, EventArgs e)
            => StopButtonPressTimer();

        private void IncrementButton_Released(object sender, EventArgs e)
            => StopButtonPressTimer();

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Default hours
            if (propertyName == DefaultHoursProperty.PropertyName)
            {
                // Init picker
                UpdateLabelText(DateTime.MinValue.AddHours(DefaultHours));
            }

            // Hours
            if (propertyName == HoursProperty.PropertyName)
            {
                // Init picker
                UpdateLabelText(DateTime.MinValue.AddHours(Hours));
            }

            // Minutes
            if (propertyName == MinutesProperty.PropertyName)
            {
                // Init picker
                UpdateLabelText(DateTime.MinValue.AddHours(Hours).AddMinutes(Minutes));
            }

            // Start text
            if (propertyName == StartTextProperty.PropertyName)
            {
                submitButton.Text = StartText;
            }

            // Start text
            if (propertyName == IsStartedProperty.PropertyName)
            {
                if (IsStarted)
                {
                    submitButton.Text = StopText;

                    DisableTimePicker();

                    StartCountDown();
                }
                else
                {
                    submitButton.Text = StartText;

                    EnableTimePicker();

                    UpdateLabelText(DateTime.MinValue.AddHours(DefaultHours).AddMinutes(DefaultMinutes));

                    StopCountDown();
                }
            }

            // Count down color
            if (propertyName == CountDownColorProperty.PropertyName)
            {
                countDownIndicator.BackgroundColor = CountDownColor;
            }
        }

        #endregion

        private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            if (!IsStarted)
                Start();
            else
                Stop();
        }

        #endregion

        private void Start()
        {
            // Start command
            if (_time.Minute == 0 && _time.Hour == 0)
                _time = _time.AddMinutes(Step);

            if (XamarinHelper.CanExecuteCommand(StartCommand, _time))
                StartCommand.Execute(_time);

            submitButton.Text = StopText;

            IsStarted = true;
        }

        private void Stop()
        {
            // Stop command
            if (XamarinHelper.CanExecuteCommand(StopCommand))
                StopCommand.Execute(null);

            submitButton.Text = StartText;

            IsStarted = false;
        }

        private void DisableTimePicker()
        {
            incrementButton.IsEnabled = false;
            decrementButton.IsEnabled = false;

            incrementButton.Opacity = 0.5;
            decrementButton.Opacity = 0.5;
        }

        private void EnableTimePicker()
        {
            incrementButton.IsEnabled = true;
            decrementButton.IsEnabled = true;

            incrementButton.Opacity = 1;
            decrementButton.Opacity = 1;
        }
    }
}