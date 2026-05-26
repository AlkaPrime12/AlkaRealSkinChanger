using System;

namespace StickFightColorCustomizer.Core
{
    public static class ModLog
    {
        private static Action<string> _info;
        private static Action<string> _warning;
        private static Action<string> _error;

        public static void BindMelon()
        {
            _info = msg =>
            {
                try
                {
                    Type logger = Type.GetType("MelonLoader.MelonLogger, MelonLoader");
                    if (logger != null)
                    {
                        logger.GetMethod("Msg", new[] { typeof(string) }).Invoke(null, new object[] { msg });
                    }
                }
                catch
                {
                }
            };
            _warning = msg =>
            {
                try
                {
                    Type logger = Type.GetType("MelonLoader.MelonLogger, MelonLoader");
                    if (logger != null)
                    {
                        logger.GetMethod("Warning", new[] { typeof(string) }).Invoke(null, new object[] { msg });
                    }
                }
                catch
                {
                }
            };
            _error = msg =>
            {
                try
                {
                    Type logger = Type.GetType("MelonLoader.MelonLogger, MelonLoader");
                    if (logger != null)
                    {
                        logger.GetMethod("Error", new[] { typeof(string) }).Invoke(null, new object[] { msg });
                    }
                }
                catch
                {
                }
            };
        }

        public static void BindBepInEx(Action<string> info, Action<string> warning, Action<string> error)
        {
            _info = info;
            _warning = warning;
            _error = error;
        }

        public static void Info(string message)
        {
            if (_info != null)
            {
                _info(message);
            }
        }

        public static void Warning(string message)
        {
            if (_warning != null)
            {
                _warning(message);
            }
        }

        public static void Error(string message)
        {
            if (_error != null)
            {
                _error(message);
            }
        }
    }
}
