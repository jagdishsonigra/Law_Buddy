import socket
def main():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_address = ('192.168.253.67', 12345)
    server_socket.bind(server_address)
    server_socket.listen(1)
    print('Waiting for connection...')

    while True:

        connection, client_address = server_socket.accept()
        print('Connected:', client_address)

        try:
            
            welcome_message = 'Hello! Welcome to your virtual lawyer.'
            connection.sendall(welcome_message.encode())

            # Chat loop
            while True:
                
                received_message = connection.recv(1024).decode()
                print('Received from Unity client:', received_message)

                if received_message.lower() == 'bye':
                    break
                    
                message_to_send = input('Enter your message: ')
                connection.sendall(message_to_send.encode())

        finally:
            connection.close()

if __name__ == "__main__":
    main()
