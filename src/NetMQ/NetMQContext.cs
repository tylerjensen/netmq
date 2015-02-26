﻿using System;
using System.Threading;
using JetBrains.Annotations;
using NetMQ.Monitoring;
using NetMQ.Sockets;
using NetMQ.zmq;

namespace NetMQ
{
    /// <summary>
    /// Context class of the NetMQ message-queuing subsystem. You should have only one context in your application process.
    /// </summary>
    public class NetMQContext : IDisposable
    {
        private readonly Ctx m_ctx;
        private int m_isClosed;

        private NetMQContext(Ctx ctx)
        {
            m_ctx = ctx;
        }

        /// <summary>
        /// Create and return a new context.
        /// </summary>
        /// <returns>the new NetMQContext</returns>
        [NotNull]
        public static NetMQContext Create()
        {
            return new NetMQContext(new Ctx());
        }

        /// <summary>
        /// Get or set the number of IO Threads in the context, default is 1.
        /// 1 is good for most cases.
        /// </summary>
        public int ThreadPoolSize
        {
            get
            {
                m_ctx.CheckDisposed();

                return m_ctx.Get(ContextOption.IOThreads);                 
            }
            set
            {
                m_ctx.CheckDisposed();

                m_ctx.Set(ContextOption.IOThreads, value);                               
            }
        }

        /// <summary>
        /// Get or set the maximum number of sockets.
        /// </summary>
        public int MaxSockets
        {
            get
            {
                m_ctx.CheckDisposed();

                return m_ctx.Get(ContextOption.MaxSockets);
            }
            set
            {
                m_ctx.CheckDisposed();

                m_ctx.Set(ContextOption.MaxSockets, value);
            }
        }

        private SocketBase CreateHandle(ZmqSocketType socketType)
        {
            m_ctx.CheckDisposed();

            return m_ctx.CreateSocket(socketType);
        }

        /// <summary>
        /// Create and return a new socket of the given socketType.
        /// </summary>
        /// <param name="socketType">a ZmqSocketType indicating the type of socket to create</param>
        /// <returns>a new socket - a subclass of NetMQSocket</returns>
        [NotNull]
        public NetMQSocket CreateSocket(ZmqSocketType socketType)
        {
            var socketHandle = CreateHandle(socketType);

            switch (socketType)
            {
                case ZmqSocketType.Pair:
                    return new PairSocket(socketHandle);
                case ZmqSocketType.Pub:
                    return new PublisherSocket(socketHandle);
                case ZmqSocketType.Sub:
                    return new SubscriberSocket(socketHandle);
                case ZmqSocketType.Req:
                    return new RequestSocket(socketHandle);
                case ZmqSocketType.Rep:
                    return new ResponseSocket(socketHandle);
                case ZmqSocketType.Dealer:
                    return new DealerSocket(socketHandle);
                case ZmqSocketType.Router:
                    return new RouterSocket(socketHandle);
                case ZmqSocketType.Pull:
                    return new PullSocket(socketHandle);
                case ZmqSocketType.Push:
                    return new PushSocket(socketHandle);
                case ZmqSocketType.Xpub:
                    return new XPublisherSocket(socketHandle);
                case ZmqSocketType.Xsub:
                    return new XSubscriberSocket(socketHandle);
                case ZmqSocketType.Stream:
                    return new StreamSocket(socketHandle);
                default:
                    throw new ArgumentOutOfRangeException("socketType");
            }
        }

        /// <summary>
        /// Create and return a new request-socket.
        /// </summary>
        /// <returns>the new RequestSocket</returns>
        [NotNull]
        public RequestSocket CreateRequestSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Req);

            return new RequestSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new response-socket.
        /// </summary>
        /// <returns>the new ResponseSocket</returns>
        [NotNull]
        public ResponseSocket CreateResponseSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Rep);

            return new ResponseSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new dealer-socket.
        /// </summary>
        /// <returns>the new DealerSocket</returns>
        [NotNull]
        public DealerSocket CreateDealerSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Dealer);

            return new DealerSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new router-socket.
        /// </summary>
        /// <returns>the new RouterSocket</returns>
        [NotNull]
        public RouterSocket CreateRouterSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Router);

            return new RouterSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new xpublisher-socket.
        /// </summary>
        /// <returns>the new XPublisherSocket</returns>
        [NotNull]
        public XPublisherSocket CreateXPublisherSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Xpub);

            return new XPublisherSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new pair-socket.
        /// </summary>
        /// <returns>the new PairSocket</returns>
        [NotNull]
        public PairSocket CreatePairSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Pair);

            return new PairSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new push-socket.
        /// </summary>
        /// <returns>the new PushSocket</returns>
        [NotNull]
        public PushSocket CreatePushSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Push);

            return new PushSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new publisher-socket.
        /// </summary>
        /// <returns>the new PublisherSocket</returns>
        [NotNull]
        public PublisherSocket CreatePublisherSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Pub);

            return new PublisherSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new pull-socket.
        /// </summary>
        /// <returns>the new PullSocket</returns>
        [NotNull]
        public PullSocket CreatePullSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Pull);

            return new PullSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new subscriber-socket.
        /// </summary>
        /// <returns>the new SubscriberSocket</returns>
        [NotNull]
        public SubscriberSocket CreateSubscriberSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Sub);

            return new SubscriberSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new xsub-socket.
        /// </summary>
        /// <returns>the new XSubscriberSocket</returns>
        [NotNull]
        public XSubscriberSocket CreateXSubscriberSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Xsub);

            return new XSubscriberSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new stream-socket.
        /// </summary>
        /// <returns>the new StreamSocket</returns>
        [NotNull]
        public StreamSocket CreateStreamSocket()
        {
            var socketHandle = CreateHandle(ZmqSocketType.Stream);

            return new StreamSocket(socketHandle);
        }

        /// <summary>
        /// Create and return a new monitor-socket that monitors the given endpoint.
        /// </summary>
        /// <param name="endpoint">a string denoting the endpoint to be monitored</param>
        /// <returns>the new NetMQMonitor</returns>
        [NotNull]
        public NetMQMonitor CreateMonitorSocket([NotNull] string endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            if (endpoint == string.Empty)
            {
                throw new ArgumentException("Unable to monitor to an empty endpoint.", "endpoint");
            }

            return new NetMQMonitor(CreatePairSocket(), endpoint);
        }

        /// <summary>
        /// Close (terminate) this context.
        /// This must not be called on a context that is already closed otherwise an ObjectDisposedException is thrown.
        /// </summary>
        public void Terminate()
        {
            if (Interlocked.CompareExchange(ref m_isClosed, 1, 0) == 0)
            {
                m_ctx.CheckDisposed();

                m_ctx.Terminate();                
            }
        }

        /// <summary>
        /// Close (or terminate) this context.
        /// </summary>
        public void Dispose()
        {
            Terminate();
        }
    }
}
