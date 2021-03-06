import {dataService} from './data.service'
import {SystemTypes} from "@/typegen";

export enum LoginStatus {
    NoStatus = 0,
    Connecting = 1,
    ConnectionError = 2,
    TokenError = 3,
    LoginError = 4
}

class LoginService {
    public state: LoginStatus = LoginStatus.NoStatus;

    public setup() {
        const token = window.localStorage.getItem('token');

        if (token) {
            this.state = LoginStatus.Connecting;
            dataService.open((data: any) => {
                if (data.type == 'error') {
                    this.state = LoginStatus.ConnectionError;
                } else if (data.type == 'open') {
                    dataService.send(SystemTypes.Login, SystemTypes.LoginTypes.Login, { token: token });
                    dataService.startPing();
                }
            });
        }

        dataService.subscribe(SystemTypes.Login, (type:number, data:any) => {
            if (type == SystemTypes.LoginTypes.Login) {
                if (data.success == true) {
                    window.localStorage.setItem("token", data.token);
                    this.state = LoginStatus.NoStatus;
                } else {
                    if (data.reason === 4001)
                        this.state = LoginStatus.LoginError;
                    else if (data.reason === 4002)
                        this.state = LoginStatus.TokenError;
                    else
                        this.state = LoginStatus.NoStatus;
                }
            }
        });
    }

    public login(username: string, password: string) {
        this.state = LoginStatus.Connecting;

        dataService.open((data: any) => {
            if (data.type == 'error') {
                this.state = LoginStatus.ConnectionError;
            } else if (data.type == 'open') {
                dataService.send(SystemTypes.Login, SystemTypes.LoginTypes.Login, { username: username, password: password });
                dataService.startPing();
            }
        });
    }

    public signup(username: string, password: string) {
        this.state = LoginStatus.Connecting;

        dataService.open((data: any) => {
            if (data.type == 'error') {
                this.state = LoginStatus.ConnectionError;
            } else if (data.type == 'open') {
                dataService.send(SystemTypes.Login, SystemTypes.LoginTypes.Signup, { username: username, password: password });
                dataService.startPing();
            }
        });
    }


    constructor() {
    }
}

export const loginService = new LoginService();
