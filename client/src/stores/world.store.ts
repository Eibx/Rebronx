import Vue from 'vue';

class Store {
    public currentNode: number = 0;


}

const WorldStore = Vue.observable(new Store());

export default WorldStore;