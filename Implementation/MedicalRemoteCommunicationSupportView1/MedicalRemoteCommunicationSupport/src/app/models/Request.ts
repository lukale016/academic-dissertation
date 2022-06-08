export class Request {
    username: string;
    name: string;
    middleName: string;
    surname: string;

    /**
     * @summary One arg copy ctor
     * @summary 4 args: username, name, middle, surname
     */
    constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.username = "";
            this.name = "";
            this.middleName = "";
            this.surname = "";
            return;
        }
        if(args.length == 1)
        {
            let request : Request = args[0] as Request;
            this.username = request.username;
            this.name = request.name;
            this.middleName = request.middleName;
            this.surname = request.surname;
            return;
        }
        if(args.length == 4)
        {
            this.username = args[0] as string;
            this.name = args[1] as string;
            this.middleName = args[2] as string;
            this.surname = args[3] as string;
            return;
        }
        console.log("Something went wrong in request ctor");
        this.username = "";
        this.name = "";
        this.middleName = "";
        this.surname = "";
    }
}