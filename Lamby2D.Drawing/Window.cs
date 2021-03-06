﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Lamby2D.Core;
using Lamby2D.Input;
using Lamby2D.Native;

namespace Lamby2D.Drawing
{
    public class Window : IDisposable
    {
        // Variables
        User32.WndProc _wndprocdelegate;
        int _width;
        int _height;
        string _title;
        WindowStyles _style;
        bool _showcursor;

        // Properties
        public IntPtr Handle { get; private set; }
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value) {
                    _width = value;
                    RECT rect = new RECT(0, 0, _width, _height);
                    User32.AdjustWindowRect(ref rect, _style, false);
                    User32.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, rect.Width, rect.Height, SetWindowPosFlags.IgnoreMove);
                }
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height != value) {
                    _height = value;
                    RECT rect = new RECT(0, 0, _width, _height);
                    User32.AdjustWindowRect(ref rect, _style, false);
                    User32.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, rect.Width, rect.Height, SetWindowPosFlags.IgnoreMove);
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (this.Handle == IntPtr.Zero) {
                    throw new ObjectDisposedException("Lamby2D.Drawing.Window instance.");
                }

                if (_title != value) {
                    _title = value;
                    User32.SetWindowText(this.Handle, value);
                }
            }
        }
        public bool ShowCursor
        {
            get { return _showcursor; }
            set
            {
                if (_showcursor != value) {
                    _showcursor = value;
                    User32.ShowCursor(value);
                }
            }
        }

        // Events
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event MouseButtonEventHandler MouseDown;
        public event MouseButtonEventHandler MouseUp;
        public event MouseMotionEventHandler MouseMotion;

        // Public
        public void Show()
        {
            User32.ShowWindow(this.Handle, ShowWindowCommands.Show);
        }
        public void Hide()
        {
            User32.ShowWindow(this.Handle, ShowWindowCommands.Hide);
        }
        public void PollMessages()
        {
            MSG msg;
            while (User32.PeekMessage(out msg, new HandleRef(this, this.Handle), 0, 0, RemoveMessage.PM_REMOVE) == true) {
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
        }
        public void Dispose()
        {
            if (this.Handle != IntPtr.Zero) {
                User32.DestroyWindow(this.Handle);
                this.Handle = IntPtr.Zero;
            }
        }

        // Private
        IntPtr WndProc(IntPtr hWnd, WindowMessages uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (uMsg == WindowMessages.SYSKEYDOWN) {
                if (wParam.ToInt32() == (int) KeyCode.F10 || wParam.ToInt32() == (int) KeyCode.Menu) {
                    if (this.KeyDown != null) {
                        this.KeyDown(this, new KeyEventArgs((KeyCode) wParam));
                    }
                    return IntPtr.Zero;
                }
            } else if (uMsg == WindowMessages.MOUSEMOVE) {
                if (this.MouseMotion != null)
                    this.MouseMotion(this, new MouseMotionEventArgs(new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
            } else if (uMsg == WindowMessages.MOUSELEAVE) {
                return IntPtr.Zero;
            } else if (uMsg == WindowMessages.CLOSE) {
                if (this.Closing != null) {
                    this.Closing(this, EventArgs.Empty);
                }
                User32.DestroyWindow(this.Handle);
            } else if (uMsg == WindowMessages.DESTROY) {
                if (this.Closed != null) {
                    this.Closed(this, EventArgs.Empty);
                }
            } else if (uMsg == WindowMessages.KEYDOWN && this.KeyDown != null) {
                this.KeyDown(this, new KeyEventArgs((KeyCode) wParam));
            } else if (uMsg == WindowMessages.KEYUP && this.KeyUp != null) {
                this.KeyUp(this, new KeyEventArgs((KeyCode) wParam));
            } else {
                if (this.MouseUp != null) {
                    if (uMsg == WindowMessages.LBUTTONUP) {
                        this.MouseUp(this, new MouseButtonEventArgs(MouseButton.Left, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    } else if (uMsg == WindowMessages.RBUTTONUP) {
                        this.MouseUp(this, new MouseButtonEventArgs(MouseButton.Right, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    } else if (uMsg == WindowMessages.MBUTTONUP) {
                        this.MouseUp(this, new MouseButtonEventArgs(MouseButton.Middle, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    }
                }
                if (this.MouseDown != null) {
                    if (uMsg == WindowMessages.LBUTTONDOWN) {
                        this.MouseDown(this, new MouseButtonEventArgs(MouseButton.Left, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    } else if (uMsg == WindowMessages.RBUTTONDOWN) {
                        this.MouseDown(this, new MouseButtonEventArgs(MouseButton.Right, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    } else if (uMsg == WindowMessages.MBUTTONDOWN) {
                        this.MouseDown(this, new MouseButtonEventArgs(MouseButton.Middle, new Point(lParam.ToInt32() & 0xFFFF, (lParam.ToInt32() >> 16) & 0xFFFF)));
                    }
                }
            }

            return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        // Constructors
        public Window()
        {
            _width = 800;
            _height = 600;
            _title = "Window";
            _style = WindowStyles.WS_OVERLAPPED | WindowStyles.WS_CAPTION | WindowStyles.WS_SYSMENU | WindowStyles.WS_MINIMIZEBOX;
            _wndprocdelegate = WndProc;
            _showcursor = true;

            WNDCLASSEX wc = new WNDCLASSEX() {
                cbSize = (uint) Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0x0002 | 0x0001,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndprocdelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                lpszMenuName = null,
                lpszClassName = typeof(Window).FullName,
            };

            if (User32.RegisterClassExW(ref wc) == 0) {
                throw new Exception("Window registration failed.");
            }

            int CW_USEDEFAULT = 0;
            unchecked { CW_USEDEFAULT = (int) 0x80000000; }

            RECT rect = new RECT(0, 0, _width, _height);
            User32.AdjustWindowRect(ref rect, _style, false);

            this.Handle = User32.CreateWindowExW(
                0,
                typeof(Window).FullName,
                this.Title,
                _style,
                CW_USEDEFAULT,
                CW_USEDEFAULT,
                rect.Width,
                rect.Height,
                IntPtr.Zero,
                IntPtr.Zero,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                IntPtr.Zero
            );

            if (this.Handle == IntPtr.Zero) {
                throw new Exception("Window creation failed.");
            }
        }
    }
}
