using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindInfo
{
    public sealed class ShakeDetector : IDisposable
    {
        private const double ShakeThreshold = 1.0;
        private Accelerometer _sensor = null;
        private SensorReadingEventArgs<AccelerometerReading> _lastReading;
        private int _shakeCount;
        private bool _shaking;

        public ShakeDetector()
        {
            if (Accelerometer.IsSupported)
            {
                var sensor = new Accelerometer();
                _sensor = sensor;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_sensor != null)
                _sensor.Dispose();

            _sensor = null;
        }

        #endregion

        private event EventHandler<ShakeDetectedEventArgs> ShakeDetectedHandler;

        public event EventHandler<ShakeDetectedEventArgs> ShakeDetected
        {
            add
            {
                if (Accelerometer.IsSupported)
                {
                    ShakeDetectedHandler += value;
                    _sensor.CurrentValueChanged += CurrentValueChanged;
                }


            }
            remove
            {
                if (Accelerometer.IsSupported)
                {
                    ShakeDetectedHandler -= value;
                    _sensor.CurrentValueChanged -= CurrentValueChanged;
                }
            }
        }

        private void CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (_sensor.State == SensorState.Ready)
            {
                var reading = e;
                try
                {
                    if (_lastReading != null)
                    {
                        if (!_shaking && CheckForShake(_lastReading.SensorReading.Acceleration, reading.SensorReading.Acceleration, ShakeThreshold) && _shakeCount >= 1)
                        {
                            //We are shaking
                            _shaking = true;
                            _shakeCount = 0;
                            OnShakeDetected();
                        }
                        else if (CheckForShake(_lastReading.SensorReading.Acceleration, reading.SensorReading.Acceleration, ShakeThreshold))
                        {
                            _shakeCount++;
                        }
                        else if (!CheckForShake(_lastReading.SensorReading.Acceleration, reading.SensorReading.Acceleration, 0.2))
                        {
                            _shakeCount = 0;
                            _shaking = false;
                        }
                    }
                    _lastReading = reading;
                }
                catch
                {
                    /* ignore errors */
                }
            }
        }

        public void Start()
        {
            if (_sensor != null)
                _sensor.Start();
        }

        public void Stop()
        {
            if (_sensor != null)
                _sensor.Stop();
        }

        private void OnShakeDetected()
        {
            if (ShakeDetectedHandler != null)
                ShakeDetectedHandler(this, new ShakeDetectedEventArgs(true));
        }

        private static bool CheckForShake(Vector3 last, Vector3 current,
                                            double threshold)
        {
            double deltaX = Math.Abs((last.X - current.X));
            double deltaY = Math.Abs((last.Y - current.Y));
            double deltaZ = Math.Abs((last.Z - current.Z));

            return (deltaX > threshold && deltaY > threshold) ||
                    (deltaX > threshold && deltaZ > threshold) ||
                    (deltaY > threshold && deltaZ > threshold);
        }
    }

}
