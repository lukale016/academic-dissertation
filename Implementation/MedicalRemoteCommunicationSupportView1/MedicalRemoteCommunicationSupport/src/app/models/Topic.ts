export class Topic {
    id: number;
    title: string;
    description: string;
    owner: string;
    comments: Comment[];

    /**
     * @summary One arg copy ctor
     * @summary 3 args: title, description, owner
     */
    constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.id = 0;
            this.title = "";
            this.description = "";
            this.owner = "";
            this.comments = [];
            return;
        }
        if(args.length == 1)
        {
            let topic : Topic = args[0] as Topic;
            this.id = topic.id;
            this.title = topic.title;
            this.description = topic.description;
            this.owner = topic.owner;
            this.comments = topic.comments;
            return;
        }
        if(args.length == 3)
        {
            this.id = 0;
            this.title = args[0] as string;
            this.description = args[1] as string;
            this.owner = args[2] as string;
            this.comments = [];
            return;
        }
        console.log("Something went wrong in topic ctor");
        this.id = 0;
        this.title = "";
        this.description = "";
        this.owner = "";
        this.comments = [];
    }
}