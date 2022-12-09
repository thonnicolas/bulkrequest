using System;
using System.Net.Sockets;

namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    class SocketErroDesc
    {
        public const String Success = "The Socket operation succeeded.";
        public const String SocketError = "An unspecified Socket error has occurred.";
        public const String Interrupted = "A blocking Socket call was canceled.";
        public const String AccessDenied = "An attempt was made to access a Socket in a way that is forbidden by its access permissions.";
        public const String Fault = "An invalid pointer address was detected by the underlying socket provider.";
        public const String InvalidArgument = "An invalid argument was supplied to a Socket member.";
        public const String TooManyOpenSockets = "There are too many open sockets in the underlying socket provider.";
        public const String WouldBlock = "An operation on a nonblocking socket cannot be completed immediately.";
        public const String InProgress = "A blocking operation is in progress.";
        public const String AlreadyInProgress = "The nonblocking Socket already has an operation in progress.";
        public const String NotSocket = "A Socket operation was attempted on a non-socket.";
        public const String DestinationAddressRequired = "A required address was omitted from an operation on a Socket.";
        public const String MessageSize = "The datagram is too long.";
        public const String ProtocolType = "The protocol type is incorrect for this Socket.";
        public const String ProtocolOption = "An unknown, invalid, or unsupported option or level was used with a Socket.";
        public const String ProtocolNotSupported = "The protocol is not implemented or has not been configured.";
        public const String SocketNotSupported = "The support for the specified socket type does not exist in this address family.";
        public const String OperationNotSupported = "The address family is not supported by the protocol family.";
        public const String ProtocolFamilyNotSupported = "The protocol family is not implemented or has not been configured.";
        public const String AddressFamilyNotSupported = "The address family specified is not supported. This error is returned if the IPv6 address family was specified and the IPv6 stack is not installed on the local machine. This error is returned if the IPv4 address family was specified and the IPv4 stack is not installed on the local machine";
        public const String AddressAlreadyInUse = "Only one use of an address is normally permitted.";
        public const String AddressNotAvailable = "The selected IP address is not valid in this context.";
        public const String NetworkDown = "The network is not available.";
        public const String NetworkUnreachable = "No route to the remote host exists.";
        public const String NetworkReset = "The application tried to set KeepAlive on a connection that has already timed out.";
        public const String ConnectionAborted = "The connection was aborted by the . NET Framework or the underlying socket provider.";
        public const String ConnectionReset = "The connection was reset by the remote peer.";
        public const String NoBufferSpaceAvailable = "No free buffer space is available for a Socket operation.";
        public const String IsConnected = "The Socket is already connected.";
        public const String NotConnected = "The application tried to send or receive data, and the Socket is not connected.";
        public const String Shutdown = "A request to send or receive data was disallowed because the Socket has already been closed.";
        public const String TimedOut = "The connection attempt timed out, or the connected host has failed to respond.";
        public const String ConnectionRefused = "The remote host is actively refusing a connection.";
        public const String HostDown = "The operation failed because the remote host is down.";
        public const String HostUnreachable = "There is no network route to the specified host.";
        public const String ProcessLimit = "Too many processes are using the underlying socket provider.";
        public const String SystemNotReady = "The network subsystem is unavailable.";
        public const String VersionNotSupported = "The version of the underlying socket provider is out of range.";
        public const String NotInitialized = "The underlying socket provider has not been initialized.";
        public const String Disconnecting = "A graceful shutdown is in progress.";
        public const String TypeNotFound = "The specified class was not found.";
        public const String HostNotFound = "No such host is known. The name is not an official host name or alias.";
        public const String TryAgain = "The name of the host could not be resolved. Try again later.";
        public const String NoRecovery = "The error is unrecoverable or the requested database cannot be located.";
        public const String NoData = "The requested name or IP address was not found on the name server.";
        public const String IOPending = "The application has initiated an overlapped operation that cannot be completed immediately.";
        public const String OperationAborted = "The overlapped operation was aborted due to the closure of the Socket.";

        public static string ErrorDescription(SocketError ErrorCode)
        {
            string desc = string.Empty;
            switch (ErrorCode)
            {
                case System.Net.Sockets.SocketError.AccessDenied:
                    desc = SocketErroDesc.AccessDenied;
                    break;
                case System.Net.Sockets.SocketError.AddressAlreadyInUse:
                    desc = SocketErroDesc.AccessDenied;
                    break;
                case System.Net.Sockets.SocketError.AddressFamilyNotSupported:
                    desc = SocketErroDesc.AddressFamilyNotSupported;
                    break;
                case System.Net.Sockets.SocketError.AddressNotAvailable:
                    desc = SocketErroDesc.AddressNotAvailable;
                    break;
                case System.Net.Sockets.SocketError.AlreadyInProgress:
                    desc = SocketErroDesc.AlreadyInProgress;
                    break;
                case System.Net.Sockets.SocketError.ConnectionAborted:
                    desc = SocketErroDesc.ConnectionAborted;
                    break;
                case System.Net.Sockets.SocketError.ConnectionRefused:
                    desc = SocketErroDesc.ConnectionRefused;
                    break;
                case System.Net.Sockets.SocketError.ConnectionReset:
                    desc = SocketErroDesc.ConnectionReset;
                    break;
                case System.Net.Sockets.SocketError.DestinationAddressRequired:
                    desc = SocketErroDesc.DestinationAddressRequired;
                    break;
                case System.Net.Sockets.SocketError.Disconnecting:
                    desc = SocketErroDesc.Disconnecting;
                    break;
                case System.Net.Sockets.SocketError.Fault:
                    desc = SocketErroDesc.Fault;
                    break;
                case System.Net.Sockets.SocketError.HostDown:
                    desc = SocketErroDesc.HostDown;
                    break;
                case System.Net.Sockets.SocketError.HostNotFound:
                    desc = SocketErroDesc.HostNotFound;
                    break;
                case System.Net.Sockets.SocketError.HostUnreachable:
                    desc = SocketErroDesc.HostUnreachable;
                    break;
                case System.Net.Sockets.SocketError.InProgress:
                    desc = SocketErroDesc.InProgress;
                    break;
                case System.Net.Sockets.SocketError.Interrupted:
                    desc = SocketErroDesc.Interrupted;
                    break;
                case System.Net.Sockets.SocketError.InvalidArgument:
                    desc = SocketErroDesc.InvalidArgument;
                    break;
                case System.Net.Sockets.SocketError.IOPending:
                    desc = SocketErroDesc.IOPending;
                    break;
                case System.Net.Sockets.SocketError.IsConnected:
                    desc = SocketErroDesc.IsConnected;
                    break;
                case System.Net.Sockets.SocketError.MessageSize:
                    desc = SocketErroDesc.MessageSize;
                    break;
                case System.Net.Sockets.SocketError.NetworkDown:
                    desc = SocketErroDesc.NetworkDown;
                    break;
                case System.Net.Sockets.SocketError.NetworkReset:
                    desc = SocketErroDesc.NetworkReset;
                    break;
                case System.Net.Sockets.SocketError.NetworkUnreachable:
                    desc = SocketErroDesc.NetworkUnreachable;
                    break;
                case System.Net.Sockets.SocketError.NoBufferSpaceAvailable:
                    desc = SocketErroDesc.NoBufferSpaceAvailable;
                    break;
                case System.Net.Sockets.SocketError.NoData:
                    desc = SocketErroDesc.NoData;
                    break;
                case System.Net.Sockets.SocketError.NoRecovery:
                    desc = SocketErroDesc.NoRecovery;
                    break;
                case System.Net.Sockets.SocketError.NotConnected:
                    desc = SocketErroDesc.NotConnected;
                    break;
                case System.Net.Sockets.SocketError.NotInitialized:
                    desc = SocketErroDesc.NotInitialized;
                    break;
                case System.Net.Sockets.SocketError.NotSocket:
                    desc = SocketErroDesc.NotSocket;
                    break;
                case System.Net.Sockets.SocketError.OperationAborted:
                    desc = SocketErroDesc.OperationAborted;
                    break;
                case System.Net.Sockets.SocketError.OperationNotSupported:
                    desc = SocketErroDesc.OperationNotSupported;
                    break;
                case System.Net.Sockets.SocketError.ProcessLimit:
                    desc = SocketErroDesc.ProcessLimit;
                    break;
                case System.Net.Sockets.SocketError.ProtocolFamilyNotSupported:
                    desc = SocketErroDesc.ProtocolFamilyNotSupported;
                    break;
                case System.Net.Sockets.SocketError.ProtocolNotSupported:
                    desc = SocketErroDesc.ProtocolNotSupported;
                    break;
                case System.Net.Sockets.SocketError.ProtocolOption:
                    desc = SocketErroDesc.ProtocolOption;
                    break;
                case System.Net.Sockets.SocketError.ProtocolType:
                    desc = SocketErroDesc.ProtocolType;
                    break;
                case System.Net.Sockets.SocketError.Shutdown:
                    desc = SocketErroDesc.Shutdown;
                    break;
                case System.Net.Sockets.SocketError.SocketError:
                    desc = SocketErroDesc.SocketError;
                    break;
                case System.Net.Sockets.SocketError.SocketNotSupported:
                    desc = SocketErroDesc.SocketNotSupported;
                    break;
                case System.Net.Sockets.SocketError.Success:
                    desc = SocketErroDesc.Success;
                    break;
                case System.Net.Sockets.SocketError.SystemNotReady:
                    desc = SocketErroDesc.SystemNotReady;
                    break;
                case System.Net.Sockets.SocketError.TimedOut:
                    desc = SocketErroDesc.TimedOut;
                    break;
                case System.Net.Sockets.SocketError.TooManyOpenSockets:
                    desc = SocketErroDesc.TooManyOpenSockets;
                    break;
                default:
                    desc = "Unknow Error";
                    break;
            }

            return desc;

        }
    }
}
