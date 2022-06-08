export class Comment {
    owner: string;
    description: string;
    isDoctorComment: boolean;

    /**
     * @summary One arg copy ctor
     * @summary 3 args: owner, description, isDoctorComment
     */
     constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.owner = "";
            this.description = "";
            this.isDoctorComment = false;
            return;
        }
        if(args.length == 1)
        {
            let comment : Comment = args[0] as Comment;
            this.owner = comment.owner;
            this.description = comment.description;
            this.isDoctorComment = comment.isDoctorComment;
            return;
        }
        if(args.length == 3)
        {
            this.owner = args[0] as string;
            this.description = args[1] as string;
            this.isDoctorComment = args[2] as boolean;
            return;
        }
        console.log("Something went wrong in comment ctor");
        this.owner = "";
        this.description = "";
        this.isDoctorComment = false;
    }
}