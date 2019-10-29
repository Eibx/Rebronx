<template>
    <div class="location">
        <div>@ westpark</div>

        <ul>
            <li><a href="">apartment building</a></li>
            <li><a href="">convenience store</a></li>
            <li><a href=""></a></li>
            <li><a href=""></a></li>
            <li><a href=""></a></li>
        </ul>

        <div id="store"></div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({ name: 'location' })
export default class Location extends Vue {

}
</script>