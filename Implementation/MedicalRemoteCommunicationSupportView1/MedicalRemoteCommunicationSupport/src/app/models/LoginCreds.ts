export class LoginCreds {
    username: string;
    password: string;

    /**
     * @summary One arg copy ctor
     * @summary 2 args: username, password
     */
    constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.username = "";
            this.password = "";
            return;
        }
        if(args.length == 1)
        {
            let creds : LoginCreds = args[0] as LoginCreds;
            this.username = creds.username;
            this.password = creds.password;
            return;
        }
        if(args.length == 2)
        {
            this.username = args[0] as string;
            this.password = args[1] as string
            return;
        }
        console.log("Something went wrong in creds ctor");
        this.username = "";
        this.password = "";
    }
}