export class ModelValidator {
        public static validate(instance: any, ignoredProperties: Array<string> = []): boolean {
            let properties = Object.getOwnPropertyNames(instance);
            let result: boolean = true;
            properties.forEach(prop => {
                if(ignoredProperties.indexOf(prop) === -1)
                    if(!this.isUsed(prop))
                        result = false;
            });
            return result;
        }

        public static isUsed(property: any): boolean {
            if(typeof property === "string" && property as string != "")
                return true;
            if(Object.prototype.toString.call(property) === "[object Date]" && property as Date != new Date())
                return true;
            if(Array.isArray(property) && (property as Array<any>).length != 0)
                return true;
            return false;
        }
}