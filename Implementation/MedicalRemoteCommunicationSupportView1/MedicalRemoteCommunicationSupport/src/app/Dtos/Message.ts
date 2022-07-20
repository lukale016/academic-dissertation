export class Message {
    public from: string;
    public content: string;
    public timeSent: Date;
    
    public constructor (from: string, content: string, timeSent: Date) {
        this.from = from;
        this.content = content;
        this.timeSent = timeSent;
    }
}