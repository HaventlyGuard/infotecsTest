
export interface DeviceSession {
    _id: string;          
    name: string;         
    startTime: string;    
    endTime: string;     
    version: string;      
}

export interface DeviceInfo {
    deviceId: string;
    userName: string;
    lastVersion: string;
    lastSeen: string;
}