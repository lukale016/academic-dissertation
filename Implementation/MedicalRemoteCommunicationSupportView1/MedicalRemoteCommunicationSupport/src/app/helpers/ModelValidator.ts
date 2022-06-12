export class ModelValidator {
        static validate(instance: any, ignoredProperties: Array<string> = []): boolean {
            let properties = Object.getOwnPropertyNames(instance);
            let result: boolean = true;
            properties.forEach(prop => {
                if(ignoredProperties.indexOf(prop) === -1)
                    if(typeof instance[prop] === "string" && instance[prop] === "") 
                        result = false;
            });
            return result;
        }
}