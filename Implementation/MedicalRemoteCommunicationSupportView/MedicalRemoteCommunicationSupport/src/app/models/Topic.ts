/// <reference path="Models.ts" />
namespace models
{
    export interface Topic {
        id: number;
        title: string;
        description: string;
        owner: string;
        comments: Comment[];
    }
}