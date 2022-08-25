import { DateTime } from "luxon";

export class Message {
    public from: string;
    public content: string;
    public timeSent: string;
    
    public constructor (from: string, content: string, timeSent: string) {
        this.from = from;
        this.content = content;
        this.timeSent = timeSent;
    }
}