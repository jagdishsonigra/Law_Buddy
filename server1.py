import socket
import wave

def main():
    # Create a socket object
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Bind the socket to a specific address and port
    server_address = ('192.168.253.67', 12345)
    server_socket.bind(server_address)

    # Listen for incoming connections
    server_socket.listen(1)
    print('Waiting for connection...')

    while True:
        # Accept a connection
        connection, client_address = server_socket.accept()
        print('Connected:', client_address)

        try:
            # Send a welcome message to Unity
            welcome_message = 'Hey ajx here! Welcome to the chat.'
            connection.sendall(welcome_message.encode())

            # Chat loop
            while True:
                # Receive a message from Unity
                received_message = connection.recv(1024).decode()
                print('Received from Unity client:', received_message)

                # If the client sends 'bye', close the connection
                if received_message.lower() == 'bye':
                    break

                # Send a message to Unity
                message_to_send = input('Enter your message: ')
                connection.sendall(message_to_send.encode())

        finally:
            # Close the connection
            connection.close()

if __name__ == "__main__":
    main()
